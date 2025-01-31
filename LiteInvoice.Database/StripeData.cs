using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database;

public class StripeData
{
	public int PaymentMethodId { get; set; }
	public string AccountId { get; set; } = default!;
	public string PublicKey { get; set; } = default!;
	public string WebhookSecret { get; set; } = default!;

	public PaymentMethod PaymentMethod { get; set; } = default!;
}

public class StripeConfiguration : IEntityTypeConfiguration<StripeData>
{
	public void Configure(EntityTypeBuilder<StripeData> builder)
	{
		builder.HasKey(x => x.PaymentMethodId);
		builder.HasOne(x => x.PaymentMethod).WithOne().HasForeignKey<StripeData>(x => x.PaymentMethodId);
	}
}