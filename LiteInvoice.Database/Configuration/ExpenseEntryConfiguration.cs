using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database.Configuration;

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
