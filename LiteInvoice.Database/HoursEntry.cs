using Database.Conventions;

namespace LiteInvoice.Database;

public class HoursEntry : BaseEntity
{
	public int ProjectId { get; set; }
	public DateOnly Date { get; set; } = DateOnly.FromDateTime(DateTime.Today);
	public string? Description { get; set; }
	public decimal Rate { get; set; }
	public decimal Hours { get; set; }
	public bool AddToInvoice { get; set; } = true;

	public Project Project { get; set; } = default!;
}
