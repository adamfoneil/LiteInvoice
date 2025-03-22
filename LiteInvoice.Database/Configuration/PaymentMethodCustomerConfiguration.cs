using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

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
