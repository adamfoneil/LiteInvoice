using Database.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ApiKey : BaseEntity
{
	public int BusinessId { get; set; }
	public string Key { get; set; } = ApiKeyUtil.Generate(32);
	public DateTime? LastUsed { get; set; }
	public bool IsEnabled { get; set; } = true;

	public Business Business { get; set; } = default!;
}

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
	public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApiKey> builder)
	{
		builder.HasIndex(e => e.Key).IsUnique();
		builder.Property(e => e.Key).HasMaxLength(32).IsRequired();
		builder.HasOne(e => e.Business).WithMany(u => u.ApiKeys).HasForeignKey(e => e.BusinessId).HasPrincipalKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
	}
}
