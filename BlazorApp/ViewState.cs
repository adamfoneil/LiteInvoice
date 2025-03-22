using LiteInvoice.Database;

namespace BlazorApp;

public class ViewState
{
	/// <summary>
	/// so that business selections can be persisted across pages
	/// </summary>
	public int BusinessId { get; set; }

	public void Initialize(IEnumerable<Business> businesses)
	{
		if (businesses.Any() && BusinessId == 0)
		{
			BusinessId = businesses.First().Id;
		}
	}
}
