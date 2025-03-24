using HashidsNet;
using LiteInvoice.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using static LiteInvoice.Database.ApplicationDbContext;

namespace BlazorApp.Pages;

public class InvoiceModel(
	ILogger<InvoiceModel> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : PageModel
{
	private readonly ILogger<InvoiceModel> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[BindProperty(SupportsGet = true, Name = "id")]
	public string InvoiceId { get; set; } = string.Empty;

	public Invoice Invoice { get; private set; } = new();
	public InvoiceData Data { get; private set; } = new();
	public PaymentMethod[] PaymentMethods { get; private set; } = [];
	public decimal Total => Data.Hours.Sum(row => row.Hours * row.Rate) + Data.Expenses.Sum(row => row.Amount);

	public async Task<IActionResult> OnGetAsync()
	{
		try
		{
			_logger.LogDebug("Viewing invoice {InvoiceId}", InvoiceId);

			using var db = _dbFactory.CreateDbContext();

			Invoice = await db.Invoices
				.Include(inv => inv.Project)
				.ThenInclude(p => p.Customer)
				.ThenInclude(c => c.Business)
				.SingleOrDefaultAsync(row => row.HashId == InvoiceId) 
				?? throw new Exception($"Not found");

			Data = JsonSerializer.Deserialize<InvoiceData>(Invoice.Data) ?? throw new Exception("Failed to deserialize invoice data");

			var businessId = Invoice.Project.Customer.BusinessId;
			
			var paymentMethods = await db.PaymentMethods
				.Where(pm => pm.BusinessId == businessId && pm.IsActive && pm.DefaultVisible)
				.ToListAsync();

			var customerId = Invoice.Project.CustomerId;

			var customerPaymentMethods = await db.PaymentMethodCustomers
				.Include(pm => pm.PaymentMethod)
				.Where(pm => pm.CustomerId == customerId && pm.PaymentMethod.IsActive)
				.Select(pm => pm.PaymentMethod)
				.ToArrayAsync();

			paymentMethods.AddRange(customerPaymentMethods.Where(pm => !paymentMethods.Any(gpm => gpm.Id == pm.Id)));
			PaymentMethods = [.. paymentMethods];

			return Page();
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error retrieving invoice {InvoiceId}", InvoiceId);
			return NotFound($"Error: {exc.Message}");
		}
	}
}
