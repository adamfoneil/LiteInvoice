using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

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

public class PaymentMethodCustomerConfiguration : IEntityTypeConfiguration<PaymentMethodCustomer>
{
	public void Configure(EntityTypeBuilder<PaymentMethodCustomer> builder)
	{
		builder.HasIndex(e => new { e.PaymentMethodId, e.CustomerId }).IsUnique();

		builder.HasOne(e => e.PaymentMethod).WithMany(e => e.PaymentMethodCustomers)
			.HasForeignKey(e => e.PaymentMethodId)
			.OnDelete(DeleteBehavior.Cascade);

		builder.HasOne(e => e.Customer).WithMany(e => e.PaymentMethodCustomers)
			.HasForeignKey(e => e.CustomerId)
			.OnDelete(DeleteBehavior.Cascade);
	}
}