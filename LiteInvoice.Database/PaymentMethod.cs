using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiteInvoice.Database;

public enum PaymentMethodType
{
	/// <summary>
	/// customer will send me a check to mailing address
	/// </summary>
	MailingAddress,
	/// <summary>
	/// customer will use my profile email to pay via services like Zelle
	/// </summary>
	BusinessEmail,
	/// <summary>
	/// customer will use my profile phone # to pay via services like Zelle
	/// </summary>
	BusinessPhone,
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
	/// PayPal.me or Venmo link, mailing address, or phone number
	/// </summary>
	public string? Data { get; set; }
	/// <summary>
	/// for example:
	/// "make checks payable to {me}"
	/// "for Zelle payments, log into your bank and pay from there"
	/// </summary>
	public string? Instructions { get; set; }
	public bool IsActive { get; set; } = true;
	/// <summary>
	/// show with all customers by default
	/// </summary>
	public bool DefaultVisible { get; set; }

	/// <summary>
	/// used in UI to determine if this payment method should be shown to the customer
	/// </summary>
	[NotMapped]
	public bool IsEnabled { get; set; }

	public Business Business { get; set; } = default!;
	public StripeData? StripeConfig { get; set; }
	public ICollection<PaymentMethodCustomer> PaymentMethodCustomers { get; set; } = [];
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
	public void Configure(EntityTypeBuilder<PaymentMethod> builder)
	{
		builder.HasOne(e => e.Business).WithMany(e => e.PaymentMethods).HasForeignKey(e => e.BusinessId).OnDelete(DeleteBehavior.Cascade);
		builder.Property(e => e.Name).HasMaxLength(50);
		builder.Property(e => e.Data).HasMaxLength(255);
		builder.Property(e => e.Instructions).HasMaxLength(255);
		builder.HasIndex(e => new { e.BusinessId, e.Name }).IsUnique();
	}
}
