using Database.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Database;

public class ApplicationUser : IdentityUser, IMailingAddress
{
	public int UserId { get; set; }
	public string? Address { get; set; }
	public string? City { get; set; }
	public string? State { get; set; }
	public string? Zip { get; set; }
	/// <summary>
	/// default hourly rate on new customers
	/// </summary>
	public decimal? HourlyRate { get; set; }

	public ICollection<Customer> Customers { get; set; } = [];
	public ICollection<PaymentMethod> Projects { get; set; } = [];
}

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
	public void Configure(EntityTypeBuilder<ApplicationUser> builder)
	{
		builder.Property(e => e.UserId).ValueGeneratedOnAdd();
		builder.HasIndex(e => e.UserId).IsUnique();
	}
}