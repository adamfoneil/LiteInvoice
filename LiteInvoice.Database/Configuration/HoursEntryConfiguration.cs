using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

public class HoursEntryConfiguration : IEntityTypeConfiguration<HoursEntry>
{
	public void Configure(EntityTypeBuilder<HoursEntry> builder)
	{
		builder.Property(e => e.Description).HasMaxLength(255);
		builder.Property(e => e.Date).HasColumnType("date");
		builder.Property(e => e.Rate).HasColumnType("money");
		builder.Property(e => e.Hours).HasColumnType("decimal(5,2)");
		builder.HasOne(e => e.Project).WithMany(e => e.Hours).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Restrict);
	}
}
