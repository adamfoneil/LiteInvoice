using LiteInvoice.Database;
using Refit;

namespace ConsoleApp;

internal interface ILiteInvoiceApi
{
	[Get("/api/businesses")]
	Task<Business[]> GetMyBusinessesAsync([Header("ApiKey")] string apiKey);

	[Post("/api/businesses")]
	Task<Business> AddBusinessAsync([Header("ApiKey")]string apiKey, [Body]Business business);

	[Put("/api/businesses/{id}")]
	Task<Business> UpdateBusinessAsync([Header("ApiKey")]string apiKey, int id, [Body]Business business);

	[Delete("/api/businesses/{id}")]
	Task DeleteBusinessAsync([Header("ApiKey")]string apiKey, int id);

	[Get("/api/customers")]
	Task<Customer> GetMyCustomersAsync([Header("ApiKey")]string apiKey);

	[Post("/api/customers")]
	Task<Customer> AddCustomerAsync([Header("ApiKey")]string apiKey, [Body]Customer customer);

	[Put("/api/customers/{id}")]
	Task<Customer> UpdateCustomerAsync([Header("ApiKey")]string apiKey, int id, [Body] Customer customer);

	[Delete("/api/customers/{id}")]
	Task DeleteCustomerAsync([Header("ApiKey")] string apiKey, int id);
}
