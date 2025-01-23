using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public class Invoice : BaseEntity
{
	public int Number { get; set; }
	public int ProjectId { get; set; }
	public DateTime Date { get; set; }
	public decimal AmountDue { get; set; }
	public string Description { get; set; } = default!;	
	public decimal AmountPaid { get; set; }

	public Project Project { get; set; } = default!;
	public ICollection<InvoiceLink> Links { get; set; } = [];
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
	public void Configure(EntityTypeBuilder<Invoice> builder)
	{		
		builder.HasIndex(e => e.Number).IsUnique();		
	}
}