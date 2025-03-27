namespace BlazorApp.Components.Pages.Dashboard;

public class PendingCharge
{
	public string Type { get; set; } = default!;
	public int CustomerId { get; set; }
	public string CustomerName { get; set; } = default!;
	public int ProjectId { get; set; }
	public string ProjectName { get; set; } = default!;
	public decimal Amount { get; set; }
}
