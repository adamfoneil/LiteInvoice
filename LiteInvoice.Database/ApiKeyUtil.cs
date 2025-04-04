﻿using System.Security.Cryptography;
using System.Text;

namespace LiteInvoice.Database;

public static class ApiKeyUtil
{
	public static string Generate(int length = 32)
	{
		const string validChars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
		var bytes = new byte[length];

		using var rng = RandomNumberGenerator.Create();
		rng.GetBytes(bytes);

		var result = new StringBuilder(length);
		foreach (var b in bytes)
		{
			result.Append(validChars[b % validChars.Length]);
		}

		return result.ToString();
	}
}