using LiteInvoice.Client.Interfaces;
using LiteInvoice.Database;
using Microsoft.Extensions.Configuration;
using Refit;
using System.Text.Json;

var config = new ConfigurationBuilder()
	.AddUserSecrets("254fb8d5-db60-4051-b43f-a4ae681fb10f")
	.AddJsonFile("appsettings.json")
	.Build();

JsonSerializerOptions jsonOptions = new() { WriteIndented = true };

var api = RestService.For<ILiteInvoiceApi>(config["BaseUrl"]);
var apiKey = config["MyApiKey"];

if (args[0].Equals("scaffold", StringComparison.OrdinalIgnoreCase))
{
	var folder = args[1];
	if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
	
	var businesses = await api.GetBusinessesAsync(apiKey);
	foreach (var biz in businesses)
	{
		var businessFolder = Path.Combine(folder, biz.Name);
		if (!Directory.Exists(businessFolder)) Directory.CreateDirectory(businessFolder);

		var json = JsonSerializer.Serialize(biz, jsonOptions);
		await File.WriteAllTextAsync(Path.Combine(businessFolder, "business.json"), json);

		if (!biz.Customers.Any()) biz.Customers.Add(new Customer()
		{
			Name = "New Customer",
			HourlyRate = 50,
			Contact = "Contact Name",
			Email = "email",
			Phone = "phone",
			Address = "address",
			City = "city",
			State = "state",
			Zip = "zip",
		});

		foreach (var customer in biz.Customers)
		{
			var customerFolder = Path.Combine(businessFolder, customer.Name);
			if (!Directory.Exists(customerFolder)) Directory.CreateDirectory(customerFolder);

			json = JsonSerializer.Serialize(customer, jsonOptions);
			await File.WriteAllTextAsync(Path.Combine(customerFolder, "customer.json"), json);

			if (!customer.Projects.Any()) customer.Projects.Add(new Project()
			{
				Name = "New Project",
				Description = "Project Description",				
				HourlyRate = customer.HourlyRate,
			});

			foreach (var prj in customer.Projects)
			{
				var projectFolder = Path.Combine(customerFolder, prj.Name);
				if (!Directory.Exists(projectFolder)) Directory.CreateDirectory(projectFolder);

				json = JsonSerializer.Serialize(prj, jsonOptions);
				await File.WriteAllTextAsync(Path.Combine(projectFolder, "project.json"), json);
			}
		}
	}

}

