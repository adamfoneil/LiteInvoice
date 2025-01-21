using Database.Conventions;
using Database.Interfaces;

namespace Database;

public class Customer : BaseEntity, IMailingAddress
{
	public int UserId { get; set; }
	public string BusinessName { get; set; } = default!;
	public string ContactName { get; set; } = default!;
	public string Email { get; set; } = default!;
	public string Phone { get; set; } = default!;
	public string? Address { get; set; } = default!;
	public string? City { get; set; } = default!;
	public string? State { get; set; } = default!;
	public string? Zip { get; set; } = default!;
	public decimal? HourlyRate { get; set; }

	public ICollection<Project> Projects { get; set; } = [];
	public ApplicationUser User { get; set; } = default!;
}
