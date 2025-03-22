using LiteInvoice.Database;

namespace BlazorApp;

public class ViewState
{
	/// <summary>
	/// so that business selections can be persisted across pages
	/// </summary>
	public int BusinessId { get; set; }

	public int UserId => CurrentUser?.UserId ?? 0;

	public ApplicationUser? CurrentUser { get; set; }


	public void Initialize(IEnumerable<Business> businesses)
	{
		if (businesses.Any() && BusinessId == 0)
		{
			BusinessId = businesses.First().Id;
		}
	}
}
