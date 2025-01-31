using ConsoleApp;
using Microsoft.Extensions.Configuration;
using Refit;

var config = new ConfigurationBuilder()
	.AddUserSecrets("254fb8d5-db60-4051-b43f-a4ae681fb10f")
	.AddJsonFile("appsettings.json")
	.Build();

var api = RestService.For<ILiteInvoiceApi>(config["BaseUrl"]);
var apiKey = config["MyApiKey"];

var businesses = await api.GetMyBusinessesAsync(apiKey);

foreach (var b in businesses) Console.WriteLine(b.Name);

Console.ReadLine();