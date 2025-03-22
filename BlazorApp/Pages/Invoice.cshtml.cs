using HashidsNet;
using LiteInvoice.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Pages;

public class InvoiceModel(
	Hashids hashids,
	IDbContextFactory<ApplicationDbContext> dbFactory) : PageModel
{
	private readonly Hashids _hashids = hashids;
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	[BindProperty(SupportsGet = true, Name = "id")]
	public string InvoiceId { get; set; } = string.Empty;

	public Invoice Invoice { get; private set; } = new();

	public async Task OnGetAsync()
    {		
		using var db = _dbFactory.CreateDbContext();
		Invoice = await db.Invoices.SingleOrDefaultAsync(row => row.HashId == InvoiceId) ?? throw new Exception("invoice not found");


	}
}
