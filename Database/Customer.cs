using Database.Conventions;
using Database.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public class Customer : BaseEntity, IMailingAddress
{
	public int UserId { get; set; }
	public string BusinessName { get; set; } = default!;
	public string ContactName { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Phone { get; set; } = default!;
	public string? Address { get; set; } = default!;
	public string? City { get; set; } = default!;
	public string? State { get; set; } = default!;
	public string? Zip { get; set; } = default!;
	public decimal? HourlyRate { get; set; }

	public ICollection<Project> Projects { get; set; } = [];
	public ApplicationUser User { get; set; } = default!;
}

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.HasOne(e => e.User).WithMany(e => e.Customers).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Restrict);
		builder.Property(e => e.BusinessName).HasMaxLength(100);
		builder.Property(e => e.ContactName).HasMaxLength(100);
		builder.Property(e => e.Email).HasMaxLength(100);
		builder.Property(e => e.Phone).HasMaxLength(100);
	}
}
