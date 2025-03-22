using Database.Conventions;
using Database.Interfaces;

namespace LiteInvoice.Database;

public class Customer : BaseEntity, IMailingAddress
{
	public int BusinessId { get; set; }
	public string Name { get; set; } = default!;
	public string Contact { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Phone { get; set; } = default!;
	public string? Address { get; set; } = default!;
	public string? City { get; set; } = default!;
	public string? State { get; set; } = default!;
	public string? Zip { get; set; } = default!;
	public decimal HourlyRate { get; set; }

	public Business Business { get; set; } = default!;
	public ICollection<Project> Projects { get; set; } = [];
	public ICollection<Payment> Payments { get; set; } = [];
	public ICollection<PaymentMethodCustomer> PaymentMethodCustomers { get; set; } = [];
}
