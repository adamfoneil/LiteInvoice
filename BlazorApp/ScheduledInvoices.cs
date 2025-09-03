using Coravel.Invocable;
using LiteInvoice.Database;
using Microsoft.EntityFrameworkCore;
using HashidsNet;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace BlazorApp;

public class ScheduledInvoices(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	ILogger<ScheduledInvoices> logger,
	IHashids hashids,
	IEmailSender? emailSender = null) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly ILogger<ScheduledInvoices> _logger = logger;
	private readonly IHashids _hashids = hashids;
	private readonly IEmailSender? _emailSender = emailSender;

	public async Task Invoke()
	{
		using var db = _dbFactory.CreateDbContext();
		db.CurrentUser = "system";

		int currentDay = DateTime.UtcNow.Day;
		int daysInMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
		int dayFromEndOfMonth = (daysInMonth - currentDay) * -1;

		_logger.LogInformation("Running scheduled invoices for day {CurrentDay} (or {DayFromEnd} from end)", currentDay, dayFromEndOfMonth);

		// Process existing project-based scheduled invoices
		await ProcessProjectScheduledInvoices(db, currentDay, dayFromEndOfMonth);

		// Process new customer-level automation
		await ProcessCustomerAutomation(db, currentDay);
	}

	private async Task ProcessProjectScheduledInvoices(ApplicationDbContext db, int currentDay, int dayFromEndOfMonth)
	{
		var scheduledInvoices = await db
			.ScheduledInvoices
			.Include(inv => inv.Project)
			.ThenInclude(p => p!.Customer)
			.ThenInclude(c => c.Business)
			.Where(row => (row.DayOfMonth == currentDay || row.DayOfMonth == dayFromEndOfMonth) && row.IsActive)
			.ToArrayAsync();

		foreach (var scheduledInvoice in scheduledInvoices)
		{
			try
			{
				_logger.LogInformation("Processing scheduled invoice for project {ProjectName}", scheduledInvoice.Project.Name);
				
				// Create invoice from the scheduled invoice logic
				var invoice = await db.CreateInvoiceAsync(scheduledInvoice.ProjectId, id => _hashids.Encode(id));
				
				_logger.LogInformation("Created invoice {InvoiceNumber} for project {ProjectName}", 
					invoice.Number, scheduledInvoice.Project.Name);

				// Send notification email if configured and AutoSend is enabled
				if (scheduledInvoice.AutoSend)
				{
					await SendInvoiceNotification(scheduledInvoice.Project.Customer.Business, 
						scheduledInvoice.Project.Customer, invoice, "scheduled");
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create scheduled invoice for project {ProjectId}", scheduledInvoice.ProjectId);
			}
		}
	}

	private async Task ProcessCustomerAutomation(ApplicationDbContext db, int currentDay)
	{
		// Get customers with automation enabled for today
		var customersToProcess = await db
			.Customers
			.Include(c => c.Business)
			.Include(c => c.AutoPostTemplate)
			.Include(c => c.Projects.Where(p => p.IsActive))
			.Where(c => c.AutoPostDayOfMonth == currentDay)
			.ToArrayAsync();

		foreach (var customer in customersToProcess)
		{
			try
			{
				_logger.LogInformation("Processing automated invoicing for customer {CustomerName}", customer.Name);
				
				if (customer.AutoPostTemplateId.HasValue && customer.AutoPostTemplate != null)
				{
					// Use template project
					var invoice = await CreateInvoiceFromTemplate(db, customer);
					if (invoice != null)
					{
						await SendInvoiceNotification(customer.Business, customer, invoice, "automated template");
					}
				}
				else
				{
					// Post pending hours/expenses for customer's active projects
					var invoices = await CreateInvoiceFromPendingWork(db, customer);
					foreach (var invoice in invoices)
					{
						await SendInvoiceNotification(customer.Business, customer, invoice, "automated pending work");
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create automated invoice for customer {CustomerId}", customer.Id);
			}
		}
	}

	private async Task<Invoice?> CreateInvoiceFromTemplate(ApplicationDbContext db, Customer customer)
	{
		if (customer.AutoPostTemplate == null)
		{
			_logger.LogWarning("Customer {CustomerName} has AutoPostTemplateId but template not found", customer.Name);
			return null;
		}

		if (!customer.AutoPostTemplate.IsTemplate)
		{
			_logger.LogWarning("Project {ProjectName} is referenced as template but IsTemplate is false", customer.AutoPostTemplate.Name);
			return null;
		}

		_logger.LogInformation("Creating invoice from template {TemplateName} for customer {CustomerName}", 
			customer.AutoPostTemplate.Name, customer.Name);
		
		// Get template hours and expenses
		var templateHours = await db.Hours
			.Where(h => h.ProjectId == customer.AutoPostTemplateId)
			.ToArrayAsync();
		
		var templateExpenses = await db.Expenses
			.Where(e => e.ProjectId == customer.AutoPostTemplateId)
			.ToArrayAsync();

		if (!templateHours.Any() && !templateExpenses.Any())
		{
			_logger.LogInformation("Template {TemplateName} has no hours or expenses to copy", customer.AutoPostTemplate.Name);
			return null;
		}

		// Find the first active, non-template project for this customer to copy template entries to
		var targetProject = customer.Projects.FirstOrDefault(p => p.IsActive && !p.IsTemplate);
		if (targetProject == null)
		{
			_logger.LogWarning("No active non-template project found for customer {CustomerName} to apply template", customer.Name);
			return null;
		}

		// Copy template hours to target project
		var currentDate = DateOnly.FromDateTime(DateTime.UtcNow);
		foreach (var templateHour in templateHours)
		{
			var newHour = new HoursEntry
			{
				ProjectId = targetProject.Id,
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
				ProjectId = targetProject.Id,
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
		var invoice = await db.CreateInvoiceAsync(targetProject.Id, id => _hashids.Encode(id), 
			$"Automated invoice from template {customer.AutoPostTemplate.Name} for {DateTime.UtcNow:MMMM yyyy}");
		
		_logger.LogInformation("Created template-based invoice {InvoiceNumber} for project {ProjectName} using template {TemplateName}", 
			invoice.Number, targetProject.Name, customer.AutoPostTemplate.Name);

		return invoice;
	}

	private async Task<List<Invoice>> CreateInvoiceFromPendingWork(ApplicationDbContext db, Customer customer)
	{
		var invoices = new List<Invoice>();
		var projectsWithPendingWork = customer.Projects
			.Where(p => p.IsActive && !p.IsTemplate)
			.ToList();

		foreach (var project in projectsWithPendingWork)
		{
			// Check if project has pending billable hours or expenses
			var hasPendingHours = await db.Hours
				.AnyAsync(h => h.ProjectId == project.Id && h.AddToInvoice);
			
			var hasPendingExpenses = await db.Expenses
				.AnyAsync(e => e.ProjectId == project.Id && e.AddToInvoice);

			if (hasPendingHours || hasPendingExpenses)
			{
				_logger.LogInformation("Creating invoice for pending work on project {ProjectName}", project.Name);
				
				var invoice = await db.CreateInvoiceAsync(project.Id, id => _hashids.Encode(id), 
					$"Automated invoice for {DateTime.UtcNow:MMMM yyyy}");
				
				_logger.LogInformation("Created automated invoice {InvoiceNumber} for project {ProjectName}", 
					invoice.Number, project.Name);

				invoices.Add(invoice);
			}
			else
			{
				_logger.LogInformation("No pending work found for project {ProjectName}", project.Name);
			}
		}

		return invoices;
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
