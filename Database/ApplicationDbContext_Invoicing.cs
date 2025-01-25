using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace Database;

public partial class ApplicationDbContext
{
	public async Task<Invoice> CreateInvoiceAsync(int projectId, string? description = null)
	{
		ChangeTracker.Clear();

		var project = await Projects
			.Include(p => p.Customer)
			.ThenInclude(c => c.Business)
			.Where(p => p.Id == projectId)
			.SingleOrDefaultAsync() ?? throw new Exception("Project not found");

		var hours = await Hours.Where(row => row.ProjectId == projectId && row.AddToInvoice).ToArrayAsync();
		var expenses = await Expenses.Where(row => row.ProjectId == projectId && row.AddToInvoice).ToArrayAsync();
		var amount = hours.Sum(row => row.Hours * row.Rate) + expenses.Sum(row => row.Amount);

		var invoice = new Invoice()
		{
			Number = project.Customer.Business.NextInvoiceNumber,
			ProjectId = projectId,
			AmountDue = amount,
			Date = DateTime.Now,
			Description = description,
			Data = JsonSerializer.Serialize(new InvoiceData() { Hours = hours, Expenses = expenses })
		};

		project.Customer.Business.NextInvoiceNumber++;
		Invoices.Add(invoice);
		Hours.RemoveRange(hours);
		Expenses.RemoveRange(expenses);
		await SaveChangesAsync();

		return invoice;
	}	

	public async Task DeleteInvoiceAsync(int invoiceId)
	{
		ChangeTracker.Clear();

		var invoice = await Invoices.SingleOrDefaultAsync(row => row.Id == invoiceId) ?? throw new Exception("Invoice not found");
		var data = JsonSerializer.Deserialize<InvoiceData>(invoice.Data) ?? throw new Exception("Couldn't deserialize invoice data");

		Hours.AddRange(data.Hours);
		Expenses.AddRange(data.Expenses);
		Invoices.Remove(invoice);
		await SaveChangesAsync();
	}

	public class InvoiceData
	{
		public HoursEntry[] Hours { get; set; } = [];
		public ExpenseEntry[] Expenses { get; set; } = [];
	}
}
