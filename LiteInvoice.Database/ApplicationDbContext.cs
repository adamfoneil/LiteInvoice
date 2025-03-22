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
	public DbSet<InvoiceLink> InvoicesLinks { get; set; }
	public DbSet<StripeData> StripeData { get; set; }
	public DbSet<Payment> Payments { get; set; }
	public DbSet<HoursEntry> Hours { get; set; }
	public DbSet<ExpenseEntry> Expenses { get; set; }	
	public DbSet<ApiKey> ApiKeys { get; set; }

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
		.Build();

	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var connectionString = Config.GetConnectionString("DefaultConnection") ?? throw new Exception("Connection string 'DefaultConnection' not found");
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseNpgsql(connectionString);
		return new ApplicationDbContext(optionsBuilder.Options);
	}
}
