namespace Database.Interfaces;

public interface IContactInfo
{
	string Name { get; set; }
	string? Contact { get; set; }
	string? Address { get; set; }
	string? City { get; set; }
	string? State { get; set; }
	string? Zip { get; set; }
	string Email { get; set; }
	string Phone { get; set; }
}
