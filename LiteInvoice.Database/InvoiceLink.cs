using Database.Conventions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database;

public class InvoiceLink : BaseEntity
{
	public int InvoiceId { get; set; }
	/// <summary>
	/// could be RazorPage, Azure blob, DigitalOcean object
	/// </summary>
	public string Url { get; set; } = default!;

	public Invoice Invoice { get; set; } = default!;
}

public class InvoiceShareConfiguration : IEntityTypeConfiguration<InvoiceLink>
{
	public void Configure(EntityTypeBuilder<InvoiceLink> builder)
	{
		builder.Property(e => e.Url).HasMaxLength(255);
		builder.HasOne(e => e.Invoice).WithMany(e => e.Links).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Restrict);
	}
}