using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using AuthExtensions;
using System.Security.Claims;

namespace LiteInvoice.Database;

public class ApplicationUser : IdentityUser, IClaimData
{
	public int UserId { get; set; }
	public string? TimeZoneId { get; set; }

	public ICollection<Business> Businesses { get; set; } = [];

	public void FromClaims(IEnumerable<Claim> claims)
	{
		Id = claims.FirstOrDefault(c => c.Type == "Id")?.Value ?? Id;
		Email = claims.FirstOrDefault(c => c.Type == "Email")?.Value ?? Email;
		UserName = claims.FirstOrDefault(c => c.Type == "UserName")?.Value ?? UserName;
		if (int.TryParse(claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var userId))
		{
			UserId = userId;
		}
		TimeZoneId = claims.FirstOrDefault(c => c.Type == "TimeZoneId")?.Value;
		if (claims.FirstOrDefault(c => c.Type == "PhoneNumber") is { } phoneClaim)
		{
			PhoneNumber = phoneClaim.Value;
		}
	}

	public IEnumerable<Claim> ToClaims()
	{
		yield return new Claim("Id", Id);
		yield return new Claim("Email", Email!);
		yield return new Claim("UserName", UserName!);
		yield return new Claim("UserId", UserId.ToString());		
		if (!string.IsNullOrEmpty(PhoneNumber)) yield return new Claim("PhoneNumber", PhoneNumber!);
		if (!string.IsNullOrEmpty(TimeZoneId)) yield return new Claim("TimeZoneId", TimeZoneId!);
	}
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.Property(e => e.UserId).ValueGeneratedOnAdd();
		builder.HasIndex(e => e.UserId).IsUnique();
	}
}