using Database.Conventions;

namespace LiteInvoice.Database;

public class ApiKey : BaseEntity
{
	public int BusinessId { get; set; }
	public string Key { get; set; } = default!;
	public DateTime? LastUsed { get; set; }
	public bool IsEnabled { get; set; } = true;

	public Business Business { get; set; } = default!;
}
