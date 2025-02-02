using System.Security.Claims;

namespace RazorPagesApp;

internal static class ClaimsProfileExtensions
{
	public static int UserId(this ClaimsPrincipal claimsPrincipal) =>
		int.Parse(claimsPrincipal.FindFirst(claim => claim.Type == ApplicationUserManager.UserIdClaimType)!.Value);
}
