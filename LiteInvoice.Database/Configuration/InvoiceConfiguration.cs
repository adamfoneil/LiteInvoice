using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
	public void Configure(EntityTypeBuilder<Invoice> builder)
	{
		builder.HasIndex(e => e.Number).IsUnique();
		builder.Property(e => e.Data).IsRequired();
		builder.Property(e => e.Description).HasMaxLength(255);
		builder.Property(e => e.HashId).HasMaxLength(64).IsRequired();
		builder.HasIndex(e => e.HashId).IsUnique();
		builder.HasOne(e => e.Project).WithMany(e => e.Invoices).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Restrict);
	}
}