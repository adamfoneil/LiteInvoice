using Database.Conventions;

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

	public StripeData? StripeConfig { get; set; }
}
