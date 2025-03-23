using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database;

public class Payment : BaseEntity
{
	public int CustomerId { get; set; }
	public int? InvoiceId { get; set; }
	public decimal Amount { get; set; }
	public DateTime Date { get; set; }
	public string? Data { get; set; }
	/// <summary>
	/// manually entered by business (i.e. ApplicationUser)
	/// </summary>
	public bool IsManual { get; set; }

	public Customer Customer { get; set; } = default!;
	public Invoice? Invoice { get; set; }
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{
		builder.Property(e => e.Data).IsRequired();
		builder.HasOne(e => e.Customer).WithMany(e => e.Payments).HasForeignKey(e => e.CustomerId).OnDelete(DeleteBehavior.Restrict);
		builder.HasOne(e => e.Invoice).WithMany(e => e.Payments).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Restrict);
	}
}
