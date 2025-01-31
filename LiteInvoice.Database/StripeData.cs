namespace LiteInvoice.Database;

public class StripeData
{
	public int PaymentMethodId { get; set; }
	public string AccountId { get; set; } = default!;
	public string PublicKey { get; set; } = default!;
	public string WebhookSecret { get; set; } = default!;

	public PaymentMethod PaymentMethod { get; set; } = default!;
}
