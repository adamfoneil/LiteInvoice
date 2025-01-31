using Microsoft.EntityFrameworkCore;

namespace LiteInvoice.Database.Configuration;

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
	public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApiKey> builder)
	{		
		builder.HasIndex(e => e.Key).IsUnique();
		builder.Property(e => e.Key).HasMaxLength(32).IsRequired();
		builder.HasOne(e => e.Business).WithMany(u => u.ApiKeys).HasForeignKey(e => e.BusinessId).HasPrincipalKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
	}
}
