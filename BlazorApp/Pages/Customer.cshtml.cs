using HashidsNet;
using LiteInvoice.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Pages;

public class CustomerModel(
	Hashids hashids,
	ILogger<CustomerModel> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : PageModel
{
	private readonly Hashids _hashids = hashids;
	private readonly ILogger<CustomerModel> _logger = logger;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[BindProperty(SupportsGet = true, Name = "id")]
	public string CustomerId { get; set; } = string.Empty;

	public Customer Customer { get; private set; } = new();
	public ILookup<int, Invoice> InvoicesByProject => Customer.Projects.SelectMany(p => p.Invoices).ToLookup(i => i.ProjectId);

	public async Task<IActionResult> OnGetAsync()
    {
		using var db = _dbFactory.CreateDbContext();

		try
		{
			var customerId = _hashids.DecodeSingle(CustomerId);

			Customer = await db.Customers
				.Include(c => c.Business)
				.Include(c => c.Projects)
				.ThenInclude(p => p.Invoices)
				.AsSplitQuery()
				.SingleOrDefaultAsync(row => row.Id == customerId)
				?? throw new Exception("Customer not found");

			return Page();
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error in OnGetAsync");
			return NotFound("Problem viewing customer");
		}
	}
}
