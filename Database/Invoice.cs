using Database.Conventions;

namespace Database;

public class Invoice : BaseEntity
{
	public int ProjectId { get; set; }
	public DateTime Date { get; set; }
	public decimal Amount { get; set; }
	public string Description { get; set; } = default!;

	public Project Project { get; set; } = default!;
}
