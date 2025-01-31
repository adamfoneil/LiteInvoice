﻿using Microsoft.AspNetCore.Identity;

namespace LiteInvoice.Database;

public class ApplicationUser : IdentityUser
{
	public int UserId { get; set; }
	public string? TimeZoneId { get; set; }

	public ICollection<Business> Businesses { get; set; } = [];
}
