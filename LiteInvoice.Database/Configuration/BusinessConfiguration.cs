using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class BusinessConfiguration : IEntityTypeConfiguration<Business>
{
	public void Configure(EntityTypeBuilder<Business> builder)
	{
		builder.HasOne(e => e.User).WithMany(u => u.Businesses).HasForeignKey(e => e.UserId).HasPrincipalKey(u => u.UserId).OnDelete(DeleteBehavior.Restrict);
		builder.HasIndex(e => new { e.UserId, e.Name }).IsUnique();
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.Property(e => e.HourlyRate).HasColumnType("money");
	}
}
