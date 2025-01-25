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

/*
 * this breaks interactive logins, so commented out for now
builder.Services.AddAuthentication(options =>
{
	options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
	options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
}).AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
{
	var key = builder.Configuration["Jwt:Secret"] ?? throw new InvalidOperationException("Jwt:Secret not found.");
	options.TokenValidationParameters = new()
	{
		ValidateIssuer = false,
		ValidateAudience = false,
		ValidateIssuerSigningKey = true,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
	};
}).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme);
*/

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

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.MapIdentityApi<ApplicationUser>();
app.MapCrudApi<ApplicationDbContext, Business>("/api/business", b => b.RequireAuthorization());

app.Run();
