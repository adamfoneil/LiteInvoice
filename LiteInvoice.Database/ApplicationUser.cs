using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace LiteInvoice.Database;

public class ApplicationUser : IdentityUser
{
	public int UserId { get; set; }
	public string? TimeZoneId { get; set; }

	public ICollection<Business> Businesses { get; set; } = [];
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.Property(e => e.UserId).ValueGeneratedOnAdd();
		builder.HasIndex(e => e.UserId).IsUnique();
	}
}