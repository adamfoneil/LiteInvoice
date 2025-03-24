using Database.Conventions;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database;

public class Payment : BaseEntity
{	
	public int InvoiceId { get; set; }
	public decimal Amount { get; set; }	
	public string? Data { get; set; }
	/// <summary>
	/// manually entered by business (i.e. ApplicationUser)
	/// </summary>
	public bool IsManual { get; set; }

	public Invoice Invoice { get; set; } = null!;
}

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
	public void Configure(EntityTypeBuilder<Payment> builder)
	{		
		builder.HasOne(e => e.Invoice).WithMany(e => e.Payments).HasForeignKey(e => e.InvoiceId).OnDelete(DeleteBehavior.Restrict);
	}
}
