using Coravel.Invocable;
using LiteInvoice.Database;
using Microsoft.EntityFrameworkCore;
using HashidsNet;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlazorApp;

public class ScheduledInvoices(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	ILogger<ScheduledInvoices> logger,
	Hashids hashids,
	IEmailSender? emailSender = null) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly ILogger<ScheduledInvoices> _logger = logger;
	private readonly Hashids _hashids = hashids;
	private readonly IEmailSender? _emailSender = emailSender;

	public async Task Invoke()
	{
		using var db = _dbFactory.CreateDbContext();
		db.CurrentUser = "system";

		int currentDay = DateTime.UtcNow.Day;
		int daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
		int dayFromEndOfMonth = (daysInMonth - currentDay) * -1;

		_logger.LogInformation("Running scheduled invoices for day {CurrentDay} (or {DayFromEnd} from end)", currentDay, dayFromEndOfMonth);

		// Process scheduled invoices from ScheduledInvoice table
		await ProcessScheduledInvoices(db, currentDay, dayFromEndOfMonth);
	}

	private async Task ProcessScheduledInvoices(ApplicationDbContext db, int currentDay, int dayFromEndOfMonth)
	{
		var scheduledInvoices = await db
			.ScheduledInvoices
			.Include(inv => inv.Project)
			.ThenInclude(p => p!.Customer)
			.ThenInclude(c => c.Business)
			.Include(inv => inv.TemplateProject)
			.Where(row => (row.DayOfMonth == currentDay || row.DayOfMonth == dayFromEndOfMonth) && row.IsActive)
			.ToArrayAsync();

		foreach (var scheduledInvoice in scheduledInvoices)
		{
			try
			{
				_logger.LogInformation("Processing scheduled invoice for project {ProjectName}", scheduledInvoice.Project.Name);
				
				Invoice? invoice = null;
				
				if (scheduledInvoice.TemplateId.HasValue && scheduledInvoice.TemplateProject != null)
				{
					// Create invoice from template
					invoice = await CreateInvoiceFromTemplate(db, scheduledInvoice);
				}
				else
				{
					// Create invoice from pending work
					invoice = await db.CreateInvoiceAsync(scheduledInvoice.ProjectId, id => _hashids.Encode(id));
				}
				
				if (invoice != null)
				{
					_logger.LogInformation("Created invoice {InvoiceNumber} for project {ProjectName}", 
						invoice.Number, scheduledInvoice.Project.Name);

					// Send notification email if configured and AutoSend is enabled
					if (scheduledInvoice.AutoSend)
					{
						await SendInvoiceNotification(scheduledInvoice.Project.Customer.Business, 
							scheduledInvoice.Project.Customer, invoice, "scheduled");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create scheduled invoice for project {ProjectId}", scheduledInvoice.ProjectId);
			}
		}
	}

	private async Task<Invoice?> CreateInvoiceFromTemplate(ApplicationDbContext db, ScheduledInvoice scheduledInvoice)
	{
		if (scheduledInvoice.TemplateProject == null)
		{
			_logger.LogWarning("ScheduledInvoice {Id} has TemplateId but template project not found", scheduledInvoice.Id);
			return null;
		}

		if (!scheduledInvoice.TemplateProject.IsTemplate)
		{
			_logger.LogWarning("Project {ProjectName} is referenced as template but IsTemplate is false", scheduledInvoice.TemplateProject.Name);
			return null;
		}

		_logger.LogInformation("Creating invoice from template {TemplateName} for project {ProjectName}", 
			scheduledInvoice.TemplateProject.Name, scheduledInvoice.Project.Name);
		
		// Get template hours and expenses
		var templateHours = await db.Hours
			.Where(h => h.ProjectId == scheduledInvoice.TemplateId)
			.ToArrayAsync();
		
		var templateExpenses = await db.Expenses
			.Where(e => e.ProjectId == scheduledInvoice.TemplateId)
			.ToArrayAsync();

		if (!templateHours.Any() && !templateExpenses.Any())
		{
			_logger.LogInformation("Template {TemplateName} has no hours or expenses to copy", scheduledInvoice.TemplateProject.Name);
			return null;
		}

		// Copy template hours to target project
		var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
		foreach (var templateHour in templateHours)
		{
			var newHour = new HoursEntry
			{
				ProjectId = scheduledInvoice.ProjectId,
				Date = currentDate,
				Description = templateHour.Description,
				Rate = templateHour.Rate,
				Hours = templateHour.Hours,
				AddToInvoice = true,
				CreatedBy = "system",
				CreatedAt = DateTime.UtcNow
			};
			db.Hours.Add(newHour);
		}

		// Copy template expenses to target project
		foreach (var templateExpense in templateExpenses)
		{
			var newExpense = new ExpenseEntry
			{
				ProjectId = scheduledInvoice.ProjectId,
				Date = currentDate,
				Description = templateExpense.Description,
				Amount = templateExpense.Amount,
				AddToInvoice = true,
				CreatedBy = "system",
				CreatedAt = DateTime.UtcNow
			};
			db.Expenses.Add(newExpense);
		}

		// Save the copied entries
		await db.SaveChangesAsync();

		// Now create the invoice from the copied entries
		var invoice = await db.CreateInvoiceAsync(scheduledInvoice.ProjectId, id => _hashids.Encode(id), 
			$"Automated invoice from template {scheduledInvoice.TemplateProject.Name} for {DateTime.UtcNow:MMMM yyyy}");
		
		_logger.LogInformation("Created template-based invoice {InvoiceNumber} for project {ProjectName} using template {TemplateName}", 
			invoice.Number, scheduledInvoice.Project.Name, scheduledInvoice.TemplateProject.Name);

		return invoice;
	}

	private async Task SendInvoiceNotification(Business business, Customer customer, Invoice invoice, string automationType)
	{
		if (_emailSender == null)
		{
			_logger.LogInformation("Email sender not configured, skipping notification for invoice {InvoiceNumber}", invoice.Number);
			return;
		}

		try
		{
			var subject = $"Automated Invoice #{invoice.Number} Created - {customer.Name}";
			var body = $@"
An {automationType} invoice has been automatically created:

Customer: {customer.Name}
Invoice Number: {invoice.Number}
Amount Due: {invoice.AmountDue:C}
Date: {invoice.Date:yyyy-MM-dd}

Please review the invoice and take any necessary action.

This is an automated notification from LiteInvoice.
";

			// Send notification to business owner
			await _emailSender.SendEmailAsync(business.Email, subject, body);
			
			_logger.LogInformation("Sent invoice notification email for invoice {InvoiceNumber} to {BusinessEmail}", 
				invoice.Number, business.Email);
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Failed to send invoice notification email for invoice {InvoiceNumber}", invoice.Number);
		}
	}
}
