﻿using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public enum PaymentMethodType
{
	/// <summary>
	/// customer will send me a check
	/// </summary>
	Offline,
	/// <summary>
	/// customer will use my profile info (email, phone number) to pay via services like Zelle
	/// </summary>
	Profile,
	/// <summary>
	/// PayPal.me, Venmo -- anything with a clickable link
	/// </summary>
	Link,
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
	public string? Link { get; set; }
	public bool IsActive { get; set; } = true;

	public ApplicationUser User { get; set; } = default!;
	public StripeData? StripeConfig { get; set; }
}

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
	public void Configure(EntityTypeBuilder<PaymentMethod> builder)
	{
		builder.HasOne(e => e.User).WithMany(e => e.PaymentMethods).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
		builder.Property(e => e.Name).HasMaxLength(100);
		builder.Property(e => e.Link).HasMaxLength(255);
	}
}
