using Database.Conventions;

namespace LiteInvoice.Database;

public class InvoiceLink : BaseEntity
{
	public int InvoiceId { get; set; }
	/// <summary>
	/// could be RazorPage, Azure blob, DigitalOcean object
	/// </summary>
	public string Url { get; set; } = default!;

	public Invoice Invoice { get; set; } = default!;
}
