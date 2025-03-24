using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace LiteInvoice.Database;

public class Invoice : BaseEntity
{
	public int Number { get; set; }
	public int ProjectId { get; set; }
	public DateTime Date { get; set; }
	public decimal AmountDue { get; set; }
	public string? Description { get; set; } = default!;
	/// <summary>
	/// json data of HoursEntry and Expense rows
	/// </summary>
	public string Data { get; set; } = default!;
	public string HashId { get; set; } = default!;

	public Project Project { get; set; } = default!;
	public ICollection<InvoiceLink> Links { get; set; } = [];
	public ICollection<Payment> Payments { get; set; } = [];

	[NotMapped]
	public decimal BalanceDue { get; set; }
}

public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
	public void Configure(EntityTypeBuilder<Invoice> builder)
	{
		builder.HasIndex(e => e.Number).IsUnique();
		builder.Property(e => e.Data).IsRequired();
		builder.Property(e => e.Description).HasMaxLength(255);
		builder.Property(e => e.HashId).HasMaxLength(64).IsRequired();
		builder.HasIndex(e => e.HashId).IsUnique();
		builder.HasOne(e => e.Project).WithMany(e => e.Invoices).HasForeignKey(e => e.ProjectId).OnDelete(DeleteBehavior.Restrict);
	}
}