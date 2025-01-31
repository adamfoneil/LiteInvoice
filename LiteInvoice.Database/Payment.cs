using Database.Conventions;

namespace LiteInvoice.Database;

public class Payment : BaseEntity
{
	public int CustomerId { get; set; }
	public int? InvoiceId { get; set; }
	public decimal Amount { get; set; }
	public DateTime Date { get; set; }
	public string? Data { get; set; }
	/// <summary>
	/// manually entered by business (i.e. ApplicationUser)
	/// </summary>
	public bool IsManual { get; set; }

	public Customer Customer { get; set; } = default!;
	public Invoice? Invoice { get; set; }
}
