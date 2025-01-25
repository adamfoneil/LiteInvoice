using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public enum PaymentMethodType
{
	/// <summary>
	/// customer will send me a check to mailing address
	/// </summary>
	ProfileAddress,
	/// <summary>
	/// customer will use my profile email to pay via services like Zelle
	/// </summary>
	ProfileEmail,
	/// <summary>
	/// customer will use my profile phone # to pay via services like Zelle
	/// </summary>
	ProfilePhone,
	/// <summary>
	/// PayPal.me, Venmo -- anything with a clickable link
	/// </summary>
	StaticLink,
	/// <summary>
	/// credit card payments accepted via Stripe
	/// </summary>
	Stripe,
	/// <summary>
	/// can we do ACH payments?
	/// </summary>
	ACH
}

public class PaymentMethod : BaseEntity
{
	public int BusinessId { get; set; }
	public PaymentMethodType Type { get; set; }
	public string Name { get; set; } = default!;
	/// <summary>
	/// PayPal.me or Venmo link
	/// </summary>
	public string? StaticLink { get; set; }
	/// <summary>
	/// for example:
	/// "make checks payable to {me}"
	/// "for Zelle payments, log into your bank and pay from there"
	/// </summary>
	public string? Instructions { get; set; }
	public bool IsActive { get; set; } = true;

	public Business Business { get; set; } = default!;
	public StripeData? StripeConfig { get; set; }
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
	public void Configure(EntityTypeBuilder<PaymentMethod> builder)
	{
		builder.HasOne(e => e.Business).WithMany(e => e.PaymentMethods).HasForeignKey(e => e.BusinessId).OnDelete(DeleteBehavior.Cascade);
		builder.Property(e => e.Name).HasMaxLength(50);
		builder.Property(e => e.StaticLink).HasMaxLength(255);
		builder.Property(e => e.Instructions).HasMaxLength(255);
		builder.HasIndex(e => new { e.BusinessId, e.Name }).IsUnique();
	}
}
