using Coravel.Invocable;
using LiteInvoice.Database;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp;

public class ScheduledInvoices(
	IDbContextFactory<ApplicationDbContext> dbFactory,
	ILogger<ScheduledInvoices> logger) : IInvocable
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;
	private readonly ILogger<ScheduledInvoices> _logger = logger;

	public async Task Invoke()
	{
		using var db = _dbFactory.CreateDbContext();

		int dayOfMonth = DateTime.DaysInMonth(DateTime.UtcNow.Year, DateTime.UtcNow.Month);
		int dayFromEndOfMonth = (dayOfMonth - DateTime.UtcNow.Day) * -1;

		var invoices = await db
			.ScheduledInvoices
			.Include(inv => inv.Project)
			.ThenInclude(p => p!.Customer)
			.ThenInclude(c => c.Business)
			.Where(row => (row.DayOfMonth == dayOfMonth || row.DayOfMonth == dayFromEndOfMonth) && row.IsActive)
			.ToArrayAsync();
	}
}
