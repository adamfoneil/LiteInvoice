using LiteInvoice.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesApp.Pages;

[Authorize]
public class BusinessModel(
	IDbContextFactory<ApplicationDbContext> dbFactory) : PageModel
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public IEnumerable<Business> Businesses { get; private set; } = [];

	public async Task OnGetAsync()
	{
		var userId = User.UserId();
		using var db = _dbFactory.CreateDbContext();
		Businesses = await db.Businesses.Where(row => row.UserId == userId).ToArrayAsync();
	}
}
