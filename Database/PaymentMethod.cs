using Database.Conventions;

namespace Database;

public class PaymentMethod : BaseEntity
{
	public int UserId { get; set; }
	public string Name { get; set; } = default!;
}
