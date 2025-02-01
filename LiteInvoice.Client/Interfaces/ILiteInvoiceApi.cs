using LiteInvoice.Database;
using Refit;

namespace LiteInvoice.Client.Interfaces;

public interface ILiteInvoiceApi
{
	[Get("/api/businesses")]
	Task<Business[]> GetBusinessesAsync([Header("ApiKey")] string apiKey);

	[Post("/api/businesses")]
	Task<Business> AddBusinessAsync([Header("ApiKey")] string apiKey, [Body] Business business);

	[Put("/api/businesses/{id}")]
	Task<Business> UpdateBusinessAsync([Header("ApiKey")] string apiKey, int id, [Body] Business business);

	[Delete("/api/businesses/{id}")]
	Task DeleteBusinessAsync([Header("ApiKey")] string apiKey, int id);

	[Get("/api/payment-methods")]
	Task<PaymentMethod> GetPaymentMethodsAsync([Header("ApiKey")] string apiKey, int businessId);

	[Post("/api/payment-methods")]
	Task<PaymentMethod> AddPaymentMethodAsync([Header("ApiKey")] string apiKey, PaymentMethod paymentMethod);

	[Put("/api/payment-methods/{id}")]
	Task<PaymentMethod> UpdatePaymentMethodAsync([Header("ApiKey")] string apiKey, int id, PaymentMethod paymentMethod);

	[Delete("/api/payment-methods/{id}")]
	Task DeletePaymentMethodAsync([Header("ApiKey")] string apiKey, int id);

	[Get("/api/customers")]
	Task<Customer> GetCustomersAsync([Header("ApiKey")] string apiKey);

	[Post("/api/customers")]
	Task<Customer> AddCustomerAsync([Header("ApiKey")] string apiKey, [Body] Customer customer);

	[Put("/api/customers/{id}")]
	Task<Customer> UpdateCustomerAsync([Header("ApiKey")] string apiKey, int id, [Body] Customer customer);

	[Delete("/api/customers/{id}")]
	Task DeleteCustomerAsync([Header("ApiKey")] string apiKey, int id);

	[Get("/api/customers/{id}/projects")]
	Task<Project> GetProjectsAsync([Header("ApiKey")] string apiKey, int id);

	[Post("/api/projects")]
	Task<Project> AddProjectAsync([Header("ApiKey")] string apiKey, Project project);

	[Put("/api/projects/{id}")]
	Task<Project> UpdateProject([Header("ApiKey")] string apiKey, int id, Project project);

	[Delete("/api/projects/{id}")]
	Task DeleteProjectAsync([Header("ApiKey")] string apiKey, int id);
}
