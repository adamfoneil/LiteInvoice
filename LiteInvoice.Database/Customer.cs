using Database.Conventions;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database;

public class Customer : BaseEntity, IContactInfo
{
	public int BusinessId { get; set; }
	public string Name { get; set; } = default!;
	public string? Contact { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Phone { get; set; } = default!;
	public string? Address { get; set; } = default!;
	public string? City { get; set; } = default!;
	public string? State { get; set; } = default!;
	public string? Zip { get; set; } = default!;
	public decimal HourlyRate { get; set; }

	public Business Business { get; set; } = default!;
	public ICollection<Project> Projects { get; set; } = [];	
	public ICollection<PaymentMethodCustomer> PaymentMethodCustomers { get; set; } = [];
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.HasIndex(e => new { e.BusinessId, e.Name }).IsUnique();
		builder.HasOne(e => e.Business).WithMany(e => e.Customers).HasForeignKey(e => e.BusinessId).OnDelete(DeleteBehavior.Restrict);
		builder.Property(e => e.Address).HasMaxLength(100);
		builder.Property(e => e.Name).HasMaxLength(100);
		builder.Property(e => e.Contact).HasMaxLength(100);
		builder.Property(e => e.Email).HasMaxLength(100);
		builder.Property(e => e.Phone).HasMaxLength(100);
		builder.Property(e => e.Zip).HasMaxLength(20);
		builder.Property(e => e.State).HasMaxLength(2);
	}
}