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
	public int UserId { get; set; }
	public PaymentMethodType Type { get; set; }
	public string Name { get; set; } = default!;
	/// <summary>
	/// PayPal.me or Venmo link
	/// </summary>
	public string? StaticLink { get; set; }
	public bool IsActive { get; set; } = true;

	public ApplicationUser User { get; set; } = default!;
	public StripeData? StripeConfig { get; set; }
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
	public void Configure(EntityTypeBuilder<PaymentMethod> builder)
	{
		builder.HasOne(e => e.User).WithMany(e => e.PaymentMethods).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
		builder.Property(e => e.Name).HasMaxLength(50);
		builder.Property(e => e.StaticLink).HasMaxLength(255);
		builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
	}
}
