namespace BlazorApp.Extensions;

public static class FormatHelper
{
	public static string ToPhoneNumber(this string? phone)
	{
		if (string.IsNullOrWhiteSpace(phone)) return string.Empty;		

		var digits = new string(phone.Where(char.IsDigit).ToArray());
		if (digits.Length == 10)
		{
			return $"({digits[..3]}) {digits.Substring(3, 3)}-{digits[6..]}";
		}
		return digits; // Return as is if not a valid 10-digit number
	}
}
