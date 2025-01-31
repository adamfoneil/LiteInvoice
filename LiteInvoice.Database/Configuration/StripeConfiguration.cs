using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class StripeConfiguration : IEntityTypeConfiguration<StripeData>
{
	public void Configure(EntityTypeBuilder<StripeData> builder)
	{
		builder.HasKey(x => x.PaymentMethodId);
		builder.HasOne(x => x.PaymentMethod).WithOne().HasForeignKey<StripeData>(x => x.PaymentMethodId);
	}
}