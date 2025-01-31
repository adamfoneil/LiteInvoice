using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class ProjectConfiguration : IEntityTypeConfiguration<Project>
{
	public void Configure(EntityTypeBuilder<Project> builder)
	{
		builder.HasIndex(e => new { e.CustomerId, e.Name }).IsUnique();
		builder.Property(e => e.Name).HasMaxLength(50).IsRequired();
		builder.Property(e => e.HourlyRate).HasColumnType("money");
		builder.HasOne(e => e.Customer).WithMany(e => e.Projects).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
	}
}
