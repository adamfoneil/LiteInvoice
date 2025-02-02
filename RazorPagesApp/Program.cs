using LiteInvoice.Database;
using HashidsNet;
using MailerSend;
using Microsoft.EntityFrameworkCore;
using RazorPagesApp;
using Scalar.AspNetCore;
using System.Text.Json;
using System.Text.Json.Serialization;
using Hydro.Configuration;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddScoped<ApplicationUserManager>();
builder.Services.Configure<JsonSerializerOptions>(options =>
{
	options.WriteIndented = true;
	options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
	options.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

builder.Services.AddHydro();

var hashidsSalt = builder.Configuration["Hashids:Salt"] ?? throw new InvalidOperationException("Hashids:Salt not found.");
var hashidsMinLength = int.Parse(builder.Configuration["Hashids:MinLength"] ?? "8");
builder.Services.AddSingleton(sp => new Hashids(hashidsSalt, hashidsMinLength));

builder.Services
	.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddUserManager<ApplicationUserManager>()	
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<MailerSendOptions>(builder.Configuration.GetSection("MailerSend"));
builder.Services.AddSingleton<MailerSendClient>();
builder.Services.AddSingleton<IEmailSender<ApplicationUser>, GenericEmailSender<ApplicationUser>>(
	sp => new GenericEmailSender<ApplicationUser>(
		"liteinvoice", 
		sp.GetRequiredService<MailerSendClient>(),
		sp.GetRequiredService<ILogger<GenericEmailSender<ApplicationUser>>>()
		));

builder.Services.AddRazorPages();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseMigrationsEndPoint();
	app.MapOpenApi();
	app.MapScalarApiReference();
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseApiKeyMiddleware();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.UseHydro(builder.Environment);

app.MapCrudApi("/api/businesses",
	(db, id) => db.Businesses.Where(row => row.Id == id),
	b => b.Id);

app.MapUserScopeQueryApi("/api/businesses",
	(db, userId) => db.Businesses.Include(b => b.PaymentMethods).Include(b => b.Customers).AsSplitQuery().Where(b => b.UserId == userId),
	b => b.Id);

app.MapCrudApi("/api/customers",
	(db, id) => db.Customers.Where(b => b.Id == id),
	c => c.BusinessId);

app.MapBusinessScopeQueryApi("/api/customers",
	(db, businessId) => db.Customers.Include(c => c.Projects).Where(c => c.BusinessId == businessId), 
	c => c.BusinessId);

app.MapCrudApi("/api/projects",
	(db, id) => db.Projects.Include(p => p.Customer).Include(p => p.Hours).Include(p => p.Expenses).Where(p => p.Id == id),
	p => p.Customer.BusinessId);

app.MapRouteQueryApi("/api/customers/{id}/projects",
	(db, id) => db.Projects.Include(p => p.Customer).Where(p => p.CustomerId == id),
	p => p.Customer.BusinessId);

app.MapCrudApi("/api/payment-methods",
	(db, id) => db.PaymentMethods.Where(row => row.Id == id),
	pm => pm.BusinessId);

app.MapBusinessScopeQueryApi("/api/payment-methods",
	(db, businessId) => db.PaymentMethods.Where(pm => pm.BusinessId == businessId),
	pm => pm.BusinessId);

app.MapCrudApi("/api/expenses",
	(db, id) => db.Expenses.Include(e => e.Project).ThenInclude(p => p.Customer).Where(row => row.Id == id),
	e => e.Project.Customer.BusinessId);

app.MapRouteQueryApi("/api/projects/{id}/expenses",
	(db, id) => db.Expenses.Include(e => e.Project).ThenInclude(p => p.Customer).Where(e => e.ProjectId == id),
	e => e.Project.Customer.BusinessId);

app.MapCrudApi("/api/hours",
	(db, id) => db.Hours.Include(h => h.Project).ThenInclude(p => p.Customer).Where(row => row.Id == id),
	h => h.Project.Customer.BusinessId);

app.MapRouteQueryApi("/api/projects/{id}/hours",
	(db, id) => db.Hours.Include(h => h.Project).ThenInclude(p => p.Customer).Where(h => h.ProjectId == id),
	h => h.Project.Customer.BusinessId);

app.Run();
