using Database;
using HashidsNet;
using MailerSend;
using Microsoft.EntityFrameworkCore;
using RazorPagesApp;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddHttpClient();

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

var hashidsSalt = builder.Configuration["Hashids:Salt"] ?? throw new InvalidOperationException("Hashids:Salt not found.");
var hashidsMinLength = int.Parse(builder.Configuration["Hashids:MinLength"] ?? "8");
builder.Services.AddSingleton(sp => new Hashids(hashidsSalt, hashidsMinLength));

builder.Services
	.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)		
	.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<MailerSendOptions>(builder.Configuration.GetSection("MailerSend"));
builder.Services.AddSingleton<MailerSendClient>();

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

app.MapCrudApi("/api/businesses", 
	(db, id) => db.Businesses.Where(row => row.Id == id), 
	b => b.Id);

app.MapCrudApi("/api/customers", 
	(db, id) => db.Customers.Where(b => b.Id == id), 
	c => c.BusinessId);

app.MapCrudApi("/api/projects", 
	(db, id) => db.Projects.Include(p => p.Customer).Where(p => p.Id == id), 
	p => p.Customer.BusinessId);

app.MapCrudApi("/api/payment-methods", 
	(db, id) => db.PaymentMethods.Where(row => row.Id == id), 
	pm => pm.BusinessId);

app.MapCrudApi("/api/expenses", 
	(db, id) => db.Expenses.Include(e => e.Project).ThenInclude(p => p.Customer).Where(row => row.Id == id), 
	e => e.Project.Customer.BusinessId);

app.MapCrudApi("/api/hours", 
	(db, id) => db.Hours.Include(h => h.Project).ThenInclude(p => p.Customer).Where(row => row.Id == id), 
	h => h.Project.Customer.BusinessId);

app.Run();
