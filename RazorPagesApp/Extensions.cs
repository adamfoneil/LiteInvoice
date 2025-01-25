using Database.Conventions;
using HashidsNet;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesApp;

public static class Extensions
{
	public static void MapCrudApi<TDbContext, TEntity>(this WebApplication app, string route, Action<RouteHandlerBuilder>? builderAction)
		where TDbContext : DbContext
		where TEntity : BaseEntity
	{		
		var hashIds = app.Services.GetRequiredService<Hashids>();		

		var builder = app.MapGet($"{route}/{{id}}", async (TDbContext context, string id) =>
		{
			var decodedId = hashIds.DecodeSingle(id);
			var entity = await context.Set<TEntity>().FindAsync(decodedId);
			if (entity is null) return Results.NotFound();
			return Results.Ok(entity);
		});

		builderAction?.Invoke(builder);

		builder = app.MapPost(route, async (TDbContext context, TEntity entity) =>
		{
			context.Set<TEntity>().Add(entity);
			await context.SaveChangesAsync();
			return Results.Created($"{route}/{hashIds.Encode(entity.Id)}", entity);			
		});

		builderAction?.Invoke(builder);

		builder = app.MapPut($"{route}/{{id}}", async (TDbContext context, string id, TEntity entity) =>
		{
			var decodedId = hashIds.DecodeSingle(id);
			if (decodedId != entity.Id) return Results.BadRequest();
			context.Set<TEntity>().Update(entity);
			await context.SaveChangesAsync();
			return Results.Ok(entity);
		});

		builderAction?.Invoke(builder);

		builder = app.MapDelete($"{route}/{{id}}", async (TDbContext context, string id) =>
		{
			var decodedId = hashIds.DecodeSingle(id);
			var entity = await context.Set<TEntity>().FindAsync(decodedId);
			if (entity is null) return Results.NotFound();
			context.Set<TEntity>().Remove(entity);
			await context.SaveChangesAsync();
			return Results.NoContent();
		});

		builderAction?.Invoke(builder);
	}
}
