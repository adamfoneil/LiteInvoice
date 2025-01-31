using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.Property(e => e.Data).IsRequired();
		builder.HasOne(e => e.Customer).WithMany(e => e.Payments).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
		builder.HasOne(e => e.Invoice).WithMany(e => e.Payments).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Restrict);
	}
}
