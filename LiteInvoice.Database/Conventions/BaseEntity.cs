using System.ComponentModel.DataAnnotations;

namespace Database.Conventions;

public class BaseEntity
{
	public int Id { get; set; }

	[MaxLength(50)]
	public string CreatedBy { get; set; } = "system";
	public DateTime CreatedAt { get; set; } = DateTime.Now;

	[MaxLength(50)]
	public string? ModifiedBy { get; set; }
	public DateTime? ModifiedAt { get; set; }
}
