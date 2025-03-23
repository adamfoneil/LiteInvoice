using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database;

public class HoursEntry : BaseEntity
{
	public int ProjectId { get; set; }
	public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
	public string? Description { get; set; }
	public decimal Rate { get; set; }
	public decimal Hours { get; set; }
	public bool AddToInvoice { get; set; } = true;

	public Project Project { get; set; } = default!;
}

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