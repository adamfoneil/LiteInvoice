using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database.Configuration;

public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
	public void Configure(EntityTypeBuilder<Customer> builder)
	{
		builder.HasIndex(e => new { e.BusinessId, e.Name }).IsUnique();
		builder.HasOne(e => e.Business).WithMany(e => e.Customers).HasForeignKey(e => e.BusinessId).OnDelete(DeleteBehavior.Restrict);
		builder.Property(e => e.Name).HasMaxLength(100);
		builder.Property(e => e.Contact).HasMaxLength(100);
		builder.Property(e => e.Email).HasMaxLength(100);
		builder.Property(e => e.Phone).HasMaxLength(100);
	}
}
