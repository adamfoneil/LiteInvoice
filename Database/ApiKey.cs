using Database.Conventions;
using Microsoft.EntityFrameworkCore;

namespace Database;

public class ApiKey : BaseEntity
{
	public int UserId { get; set; }
	public string Key { get; set; } = ApiKeyUtil.Generate(32);
	public DateTime? LastUsed { get; set; }
	public bool IsEnabled { get; set; } = true;

	public ApplicationUser User { get; set; } = default!;
}

public class ApiKeyConfiguration : IEntityTypeConfiguration<ApiKey>
{
	public void Configure(Microsoft.EntityFrameworkCore.Metadata.Builders.EntityTypeBuilder<ApiKey> builder)
	{		
		builder.HasOne(e => e.User).WithMany(u => u.ApiKeys).HasForeignKey(e => e.UserId).HasPrincipalKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
	}
}
