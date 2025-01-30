using Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesApp;

public class ApiKeyMiddleware(RequestDelegate next)
{
	private readonly RequestDelegate _next = next;	

	public async Task InvokeAsync(HttpContext context, IDbContextFactory<ApplicationDbContext> dbFactory)
	{
		if (!context.Request.Path.StartsWithSegments("/api"))
		{
			await _next(context);
			return;
		}

		var endpoint = context.GetEndpoint();
		if (endpoint?.Metadata.GetMetadata<AllowAnonymousAttribute>() is not null)
		{
			await _next(context);
			return;
		}

		if (!context.Request.Headers.TryGetValue("ApiKey", out var apiKey))
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		using var db = dbFactory.CreateDbContext();
		var useKey = apiKey.First();
		var keyRow = await db.ApiKeys
			.Include(ak => ak.Business)
			.ThenInclude(b => b.User)
			.FirstOrDefaultAsync(u => u.Key == useKey);

		if (keyRow is null || !keyRow.IsEnabled)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			return;
		}

		context.Items["business"] = keyRow.Business;

		keyRow.LastUsed = DateTime.UtcNow;
		await db.SaveChangesAsync();

		await _next(context);
	}
}

public static class ApiKeyMiddlewareExtensions
{
	public static IApplicationBuilder UseApiKeyMiddleware(this IApplicationBuilder builder) =>
		builder.UseMiddleware<ApiKeyMiddleware>();
}
