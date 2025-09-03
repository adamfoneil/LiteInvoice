using Coravel.Invocable;
using LiteInvoice.Database;
using Microsoft.EntityFrameworkCore;
using HashidsNet;

namespace BlazorApp;

public class ScheduledInvoices(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	ILogger<ScheduledInvoices> logger,
	IHashids hashids) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly ILogger<ScheduledInvoices> _logger = logger;
	private readonly IHashids _hashids = hashids;

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
					await CreateInvoiceFromTemplate(db, customer);
				}
				else
				{
					// Post pending hours/expenses for customer's active projects
					await CreateInvoiceFromPendingWork(db, customer);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to create automated invoice for customer {CustomerId}", customer.Id);
			}
		}
	}

	private async Task CreateInvoiceFromTemplate(ApplicationDbContext db, Customer customer)
	{
		if (customer.AutoPostTemplate == null)
		{
			_logger.LogWarning("Customer {CustomerName} has AutoPostTemplateId but template not found", customer.Name);
			return;
		}

		_logger.LogInformation("Creating invoice from template {TemplateName} for customer {CustomerName}", 
			customer.AutoPostTemplate.Name, customer.Name);
		
		// For template-based invoices, we need to copy template entries to a regular project first
		// or create an invoice based on template structure
		// This is a simplified implementation - in practice, you might want to copy template hours/expenses
		// to a regular project and then invoice from there
		
		_logger.LogInformation("Template-based invoice creation not yet implemented for customer {CustomerName}", customer.Name);
	}

	private async Task CreateInvoiceFromPendingWork(ApplicationDbContext db, Customer customer)
	{
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
			}
			else
			{
				_logger.LogInformation("No pending work found for project {ProjectName}", project.Name);
			}
		}
	}
}
