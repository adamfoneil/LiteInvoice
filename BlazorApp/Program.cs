using AuthExtensions;
using BlazorApp;
using BlazorApp.Components;
using BlazorApp.Components.Account;
using CoreNotify.MailerSend.Extensions;
using HashidsNet;
using LiteInvoice.Database;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Radzen;
using System.Globalization;

var builder = WebApplication.CreateBuilder(args);

var defaultCulture = new CultureInfo("en-US");
CultureInfo.DefaultThreadCurrentCulture = defaultCulture;
CultureInfo.DefaultThreadCurrentUICulture = defaultCulture;

// Add services to the container.
builder.Services.AddRazorComponents()
	.AddInteractiveServerComponents();

builder.Services.AddHttpClient();
builder.Services.AddRadzenComponents();
builder.Services.AddRazorPages();
builder.Services.AddScoped<CurrentUserAccessor<ApplicationDbContext, ApplicationUser>>();

builder.Services.AddSingleton(sp => new Hashids(
	builder.Configuration["Hashids:Salt"], 
	minHashLength: int.TryParse(builder.Configuration["Hashids:MinLength"], out var value) ? value : 6));
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<IdentityUserAccessor>();
builder.Services.AddScoped<IdentityRedirectManager>();
builder.Services.AddScoped<AuthenticationStateProvider, IdentityRevalidatingAuthenticationStateProvider>();
builder.Services.AddScoped<ViewState>();

builder.Services.AddAuthentication(options =>
	{
		options.DefaultScheme = IdentityConstants.ApplicationScheme;
		options.DefaultSignInScheme = IdentityConstants.ExternalScheme;
	})
	.AddIdentityCookies();

var connectionName = builder.Configuration.GetValue<string>("ConnectionName") ?? "DefaultConnection";
builder.Services.Configure<DbConnection>(info => info.ConnectionName = connectionName);
var connectionString = builder.Configuration.GetConnectionString(connectionName) ?? throw new InvalidOperationException($"Connection string '{connectionName}' not found.");
builder.Services.AddDbContextFactory<ApplicationDbContext>(options => options.UseNpgsql(connectionString), ServiceLifetime.Singleton);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddIdentityCore<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
	.AddEntityFrameworkStores<ApplicationDbContext>()
	.AddSignInManager()
	.AddDefaultTokenProviders();

builder.Services.AddCoreNotifyGenericEmailSender<ApplicationUser>("liteinvoice", builder.Configuration);

builder.Services.AddScoped<CurrentUserAccessor<ApplicationDbContext, ApplicationUser>>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	var dbFactory = app.Services.GetRequiredService<IDbContextFactory<ApplicationDbContext>>();
	using var db = dbFactory.CreateDbContext();
	db.Database.Migrate();
	app.UseMigrationsEndPoint();
}
else
{
	app.UseExceptionHandler("/Error", createScopeForErrors: true);
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorPages();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode();

// Add additional endpoints required by the Identity /Account Razor components.
app.MapAdditionalIdentityEndpoints();

app.Run();
