using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public class ExpenseEntry : BaseEntity
{
	public int ProjectId { get; set; }
	public DateOnly Date { get; set; }
	public string? Description { get; set; }
	public decimal Amount { get; set; }
	public bool AddToInvoice { get; set; } = true;

	public Project Project { get; set; } = default!;
}

public class ExpenseEntryConfiguration : IEntityTypeConfiguration<ExpenseEntry>
{
	public void Configure(EntityTypeBuilder<ExpenseEntry> builder)
	{
		builder.Property(e => e.Description).HasMaxLength(255);
		builder.Property(e => e.Date).HasColumnType("date");
		builder.Property(e => e.Amount).HasColumnType("money");
		builder.HasOne(e => e.Project).WithMany(e => e.Expenses).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Restrict);
	}
}
