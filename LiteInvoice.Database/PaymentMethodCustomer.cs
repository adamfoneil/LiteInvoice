using Database.Conventions;

namespace LiteInvoice.Database;

/// <summary>
/// defines which payment methods to show with which customer
/// </summary>
public class PaymentMethodCustomer : BaseEntity
{
	public int PaymentMethodId { get; set; }
	public int CustomerId { get; set; }
	public bool IsEnabled { get; set; }

	public PaymentMethod PaymentMethod { get; set; } = default!;
	public Customer Customer { get; set; } = default!;
}
