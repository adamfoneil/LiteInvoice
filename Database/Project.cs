using Database.Conventions;

namespace Database;

public class Project : BaseEntity
{
	public int CustomerId { get; set; }
	public string Name { get; set; } = default!;
	public string Description { get; set; } = default!;
	public decimal? HourlyRate { get; set; }

	public Customer Customer { get; set; } = default!;
	public ICollection<Invoice> Invoices { get; set; } = [];
}
