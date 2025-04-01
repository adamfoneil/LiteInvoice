using Database.Conventions;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace LiteInvoice.Database;

public partial class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
	public string? CurrentUser { get; set; }

	public DbSet<Business> Businesses { get; set; }
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Project> Projects { get; set; }
	public DbSet<Invoice> Invoices { get; set; }
	public DbSet<PaymentMethod> PaymentMethods { get; set; }
	public DbSet<PaymentMethodCustomer> PaymentMethodCustomers { get; set; }
	public DbSet<InvoiceLink> InvoicesLinks { get; set; }
	public DbSet<StripeData> StripeData { get; set; }
	public DbSet<Payment> Payments { get; set; }
	public DbSet<HoursEntry> Hours { get; set; }
	public DbSet<ExpenseEntry> Expenses { get; set; }	
	public DbSet<ApiKey> ApiKeys { get; set; }
	public DbSet<ScheduledInvoice> ScheduledInvoices { get; set; }

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);
		builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

		foreach (var entityType in builder.Model.GetEntityTypes())
		{
			if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
			{
				builder.Entity(entityType.ClrType)
					.Property(nameof(BaseEntity.CreatedAt))
					.HasColumnType("timestamp without time zone");

				builder.Entity(entityType.ClrType)
					.Property(nameof(BaseEntity.ModifiedAt))
					.HasColumnType("timestamp without time zone");
			}
		}
	}

	public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
	{
		foreach (var entry in ChangeTracker.Entries<BaseEntity>())
		{
			if (entry.State == EntityState.Modified)
			{
				entry.Entity.ModifiedBy = CurrentUser ?? "system";
				entry.Entity.ModifiedAt = DateTime.Now;
			}			
		}

		return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
	}
}

public class AppDbFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	private static IConfiguration Config => new ConfigurationBuilder()
		.AddJsonFile("appsettings.json", optional: false)
		.AddUserSecrets("f5546b2a-c3cb-490b-b6c8-dcf2a719c8d5")
		.Build();

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var connectionName = args.Length == 1 ? args[0] : Config.GetValue<string>("ConnectionName") ?? "DefaultConnection";
		var connectionString = Config.GetConnectionString(connectionName) ?? throw new Exception($"Connection string '{connectionName}' not found");
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseNpgsql(connectionString);
		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
