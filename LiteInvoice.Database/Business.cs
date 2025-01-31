using Database.Conventions;
using Database.Interfaces;

namespace LiteInvoice.Database;

public class Business : BaseEntity, IMailingAddress
{
	public int UserId { get; set; }
	public string Name { get; set; } = default!;
	public string? ContactName { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Zip { get; set; }
	public int NextInvoiceNumber { get; set; } = 1000;
	public decimal? HourlyRate { get; set; }

	public ApplicationUser User { get; set; } = default!;
	public ICollection<Customer> Customers { get; set; } = [];
	public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
	public ICollection<ApiKey> ApiKeys { get; set; } = [];
}
