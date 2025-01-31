using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class InvoiceShareConfiguration : IEntityTypeConfiguration<InvoiceLink>
{
	public void Configure(EntityTypeBuilder<InvoiceLink> builder)
	{
		builder.Property(e => e.Url).HasMaxLength(255);
		builder.HasOne(e => e.Invoice).WithMany(e => e.Links).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Restrict);
	}
}