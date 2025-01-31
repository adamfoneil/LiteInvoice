using Database.Conventions;

namespace LiteInvoice.Database;

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
