using Database.Conventions;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database;

public class Business : BaseEntity, IContactInfo
{
	public int UserId { get; set; }
	public string Name { get; set; } = default!;
	public string? Contact { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Zip { get; set; }
	public string Phone { get; set; } = default!;
	public string Email { get; set; } = default!;
	public int NextInvoiceNumber { get; set; } = 1000;
	public decimal? HourlyRate { get; set; }

	public ApplicationUser User { get; set; } = default!;
	public ICollection<Customer> Customers { get; set; } = [];
	public ICollection<PaymentMethod> PaymentMethods { get; set; } = [];
	public ICollection<ApiKey> ApiKeys { get; set; } = [];
}

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
	public void Configure(EntityTypeBuilder<Business> builder)
	{
		builder.HasOne(e => e.User).WithMany(u => u.Businesses).HasForeignKey(e => e.UserId).HasPrincipalKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.Property(e => e.Contact).HasMaxLength(50);
		builder.Property(e => e.Address).HasMaxLength(100);
		builder.Property(e => e.City).HasMaxLength(50);
		builder.Property(e => e.State).HasMaxLength(50);
		builder.Property(e => e.Email).HasMaxLength(50);
		builder.Property(e => e.Zip).HasMaxLength(20);
		builder.Property(e => e.Phone).HasMaxLength(20);
		builder.Property(e => e.HourlyRate).HasColumnType("money");
	}
}