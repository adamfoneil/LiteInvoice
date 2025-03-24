using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LiteInvoice.Database;

public partial class ApplicationDbContext
{
	public async Task<(HoursEntry[] Hours, ExpenseEntry[] Expenses, decimal Total)> GetInvoiceDataAsync(int projectId)
	{
		var hours = await Hours
			.Where(row => row.ProjectId == projectId && row.AddToInvoice)
			.ToArrayAsync();

		foreach (var item in hours) item.Project = null!;		

		var expenses = await Expenses
			.Where(row => row.ProjectId == projectId && row.AddToInvoice)
			.ToArrayAsync();

		foreach (var item in expenses) item.Project = null!;

		var amount = hours.Sum(row => row.Hours * row.Rate) + expenses.Sum(row => row.Amount);

		return (hours, expenses, amount);
	}

	private static JsonSerializerOptions DefaultSettings => new()
	{
		WriteIndented = true,
		ReferenceHandler = ReferenceHandler.IgnoreCycles
	};

	public async Task<Invoice> CreateInvoiceAsync(int projectId, Func<int, string> hashMethod, string? description = null)
	{
		ChangeTracker.Clear();

		var project = await Projects
			.Include(p => p.Customer)
			.ThenInclude(c => c.Business)
			.Where(p => p.Id == projectId)
			.SingleOrDefaultAsync() ?? throw new Exception("Project not found");

		if (project.IsTemplate) throw new Exception("Cannot create invoice from template project");

		var (hours, expenses, amount) = await GetInvoiceDataAsync(projectId);

		var invoice = new Invoice()
		{
			Number = project.Customer.Business.NextInvoiceNumber,
			ProjectId = projectId,
			AmountDue = amount,
			Date = DateTime.UtcNow,
			Description = description,
			HashId = "dummy",
			Data = JsonSerializer.Serialize(new InvoiceData() { Hours = hours, Expenses = expenses }, DefaultSettings)
		};

		project.Customer.Business.NextInvoiceNumber++;
		Invoices.Add(invoice);
		Hours.RemoveRange(hours);
		Expenses.RemoveRange(expenses);
		await SaveChangesAsync();

		await Invoices.Where(row => row.Id == invoice.Id)
			.ExecuteUpdateAsync(row => row.SetProperty(i => i.HashId, hashMethod(invoice.Id)));

		return invoice;
	}

	public async Task DeleteInvoiceAsync(int invoiceId)
	{
		ChangeTracker.Clear();

		var invoice = await Invoices.SingleOrDefaultAsync(row => row.Id == invoiceId) ?? throw new Exception("Invoice not found");
		var data = JsonSerializer.Deserialize<InvoiceData>(invoice.Data) ?? throw new Exception("Couldn't deserialize invoice data");

		// ensure inserts
		foreach (var item in data.Hours) item.Id = 0;
		foreach (var item in data.Expenses) item.Id = 0;

		Hours.UpdateRange(data.Hours);
		Expenses.UpdateRange(data.Expenses);
		Invoices.Remove(invoice);
		await SaveChangesAsync();
	}

	public class InvoiceData
	{
		public HoursEntry[] Hours { get; set; } = [];
		public ExpenseEntry[] Expenses { get; set; } = [];
	}
}
