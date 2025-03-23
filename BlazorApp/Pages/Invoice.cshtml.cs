using HashidsNet;
using LiteInvoice.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

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

			return Page();
		}
		catch (Exception exc)
		{
			_logger.LogError(exc, "Error retrieving invoice {InvoiceId}", InvoiceId);
			return NotFound($"Error: {exc.Message}");
		}
	}
}
