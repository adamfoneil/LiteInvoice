using Database.Conventions;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public class Business : BaseEntity, IMailingAddress
{
	public int UserId { get; set; }
	public string Name { get; set; } = default!;
	public string? ContactName { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Zip { get; set; }
	public int NextInvoiceNumber { get; set; } = 1000;
	public decimal? HourlyRate { get; set; }

	public ICollection<Customer> Customers { get; set; } = [];
	public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
}

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
	public void Configure(EntityTypeBuilder<Business> builder)
	{
		builder.HasOne<ApplicationUser>().WithMany(u => u.Businesses).HasForeignKey(e => e.UserId).HasPrincipalKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.Property(e => e.HourlyRate).HasColumnType("money");
	}
}
