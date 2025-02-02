using LiteInvoice.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesApp.Pages;

[Authorize]
public class BusinessModel(
	IDbContextFactory<ApplicationDbContext> dbFactory) : PageModel
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;	

	public async Task OnGetAsync()
	{
		var userId = User.UserId();

		using var db = _dbFactory.CreateDbContext();

	}
}
