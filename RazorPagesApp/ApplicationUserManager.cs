using Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace RazorPagesApp;

public class ApplicationUserManager(
	IUserStore<ApplicationUser> store, 
	IOptions<IdentityOptions> optionsAccessor, 
	IPasswordHasher<ApplicationUser> passwordHasher, 
	IEnumerable<IUserValidator<ApplicationUser>> userValidators, 
	IEnumerable<IPasswordValidator<ApplicationUser>> passwordValidators, 
	ILookupNormalizer keyNormalizer, 
	IdentityErrorDescriber errors, 
	IServiceProvider services, 
	ILogger<UserManager<ApplicationUser>> logger,
	IDbContextFactory<ApplicationDbContext> dbFactory) : UserManager<ApplicationUser>(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
{
	private readonly IDbContextFactory<ApplicationDbContext> _dbFactory = dbFactory;

	public override async Task<IdentityResult> CreateAsync(ApplicationUser user, string password)
	{
		var result = await base.CreateAsync(user, password);

		if (result == IdentityResult.Success)
		{
			using var db = _dbFactory.CreateDbContext();
			var userId = await db.Users.SingleAsync(row => row.UserName == user.UserName);

			db.Businesses.Add(new Business()
			{
				Name = user.UserName!,
				UserId = userId.UserId,
				ApiKeys = [ new() ]
			});

			await db.SaveChangesAsync();
		}

		return result;
	}
}
