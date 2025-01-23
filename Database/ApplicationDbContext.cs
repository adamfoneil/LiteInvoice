using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
{
	public DbSet<Customer> Customers { get; set; }
	public DbSet<Project> Projects { get; set; }
	public DbSet<Invoice> Invoices { get; set; }
	public DbSet<PaymentMethod> PaymentMethods { get; set; }
	public DbSet<InvoiceLink> InvoicesLinks { get; set; }
	public DbSet<StripeData> StripeData { get; set; }
}
