using Database.Conventions;

namespace LiteInvoice.Database;

public class ExpenseEntry : BaseEntity
{
	public int ProjectId { get; set; }
	public DateOnly Date { get; set; }
	public string? Description { get; set; }
	public decimal Amount { get; set; }
	public bool AddToInvoice { get; set; } = true;

	public Project Project { get; set; } = default!;
}
