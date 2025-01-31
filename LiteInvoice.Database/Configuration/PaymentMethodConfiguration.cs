using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
	public void Configure(EntityTypeBuilder<PaymentMethod> builder)
	{
		builder.HasOne(e => e.Business).WithMany(e => e.PaymentMethods).HasForeignKey(e => e.BusinessId).OnDelete(DeleteBehavior.Cascade);
		builder.Property(e => e.Name).HasMaxLength(50);
		builder.Property(e => e.StaticLink).HasMaxLength(255);
		builder.Property(e => e.Instructions).HasMaxLength(255);
		builder.HasIndex(e => new { e.BusinessId, e.Name }).IsUnique();
	}
}
