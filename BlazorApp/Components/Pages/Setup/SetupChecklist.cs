using LiteInvoice.Database;
using Microsoft.EntityFrameworkCore;

namespace BlazorApp.Components.Pages.Setup;

public class SetupChecklist
{
	private static Item[] AllItems(int userId) => [
		new()
		{
			Text = "Create a business",
			Link = "/Setup/Business",
			Description = "Set your business and contact info that appears on all your invoices",
			CompleteIf = async (db) => await db.Businesses.Where(b => b.UserId == userId).AnyAsync()
		},
		new()
		{
			Text = "Create one or more payment methods",
			Link = "/Setup/PaymentMethods",
			Description = "Define one or more ways clients can pay you",
			CompleteIf = async (db) => await db.PaymentMethods.Include(pm => pm.Business).Where(pm => pm.Business.UserId == userId).AnyAsync()
		},
		new()
		{
			Text = "Create customers",
			Link = "/Setup/Customers",
			Description = "Add your clients to keep track of who you're invoicing",
			CompleteIf = async (db) => await db.Customers.Include(c => c.Business).Where(c => c.Business.UserId == userId).AnyAsync()
		},
		new()
		{
			Text = "Create projects",
			Link = "/Setup/Projects",
			Description = "Define the work you're doing for each client",
			CompleteIf = async (db) => await db.Projects.Include(p => p.Customer).ThenInclude(c => c.Business).Where(p => p.Customer.Business.UserId == userId).AnyAsync()
		}
	];

	public async Task<IEnumerable<Item>> GetIncompleteItemsAsync(ApplicationDbContext db, int userId)
	{		
		List<Item> results = [];

		foreach (var item in AllItems(userId))
		{
			if (!await item.CompleteIf(db)) results.Add(item);			
		}

		return results;
	}

	public class Item
	{
		public required string Text { get; set; } 
		public required string Link { get; set; }
		public required string Description { get; set; }
		public required Func<ApplicationDbContext, Task<bool>> CompleteIf { get; set; }
		public bool IsCompleted { get; set; }
	}
}
