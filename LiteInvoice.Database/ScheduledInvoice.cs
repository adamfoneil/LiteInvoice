using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database;

public class ScheduledInvoice : BaseEntity
{
	/// <summary>
	/// project for which invoice is created
	/// </summary>
	public int ProjectId { get; set; }
	/// <summary>
	/// positive numbers count from the start of month, negative from the end
	/// </summary>
	public int DayOfMonth { get; set; }
	/// <summary>
	/// if this is a template invoice, then use this as the base for the scheduled invoice	
	/// </summary>
	public int? TemplateId { get; set; }	
	public bool IsActive { get; set; } = true;

	public Project Project { get; set; } = default!;
	public Project TemplateProject { get; set; } = default!;
}

public class ScheduledInvoiceConfiguration : IEntityTypeConfiguration<ScheduledInvoice>
{
	public void Configure(EntityTypeBuilder<ScheduledInvoice> builder)
	{
		builder.HasIndex(e => e.ProjectId).IsUnique();

		builder.HasOne(e => e.Project)
			.WithMany(p => p.ScheduledInvoices)
			.HasForeignKey(e => e.ProjectId)
			.OnDelete(DeleteBehavior.Cascade); 

		builder.HasOne(e => e.TemplateProject)
			.WithMany(p => p.ScheduledTemplateInvoices)
			.HasForeignKey(e => e.TemplateId)
			.OnDelete(DeleteBehavior.Restrict);
	}
}