namespace LiteInvoice.Database.Interfaces;

public interface IPaymentWebhook
{
	string InvoiceId { get; set; }
}
