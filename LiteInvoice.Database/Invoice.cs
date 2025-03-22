using Database.Conventions;

namespace LiteInvoice.Database;

public class Invoice : BaseEntity
{
	public int Number { get; set; }
	public int ProjectId { get; set; }
	public DateTime Date { get; set; }
	public decimal AmountDue { get; set; }
	public string? Description { get; set; } = default!;
	/// <summary>
	/// json data of HoursEntry and Expense rows
	/// </summary>
	public string Data { get; set; } = default!;
	public string HashId { get; set; } = default!;

	public Project Project { get; set; } = default!;
	public ICollection<InvoiceLink> Links { get; set; } = [];
	public ICollection<Payment> Payments { get; set; } = [];	
}
