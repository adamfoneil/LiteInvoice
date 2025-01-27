using Database;
using Database.Conventions;
using Microsoft.EntityFrameworkCore;

namespace RazorPagesApp;

public static class Extensions
{
	public static void MapCrudApi<TEntity>(this WebApplication app, string route, Func<ApplicationDbContext, int, IQueryable<TEntity>> query, Func<TEntity, int> getBusinessId, Action<RouteHandlerBuilder>? builderAction = null)		
		where TEntity : BaseEntity
	{		
		var builder = app.MapGet($"{route}/{{id}}", async (HttpContext httpContext, ApplicationDbContext db, int id) =>
		{
			var business = httpContext.Items["business"] as Business ?? throw new Exception("business not found");
			var entity = await query.Invoke(db, id).AsSplitQuery().SingleOrDefaultAsync();

			if (entity is null) return Results.NotFound();

			var businessId = getBusinessId(entity);
			if (businessId != business.Id) return Results.Forbid();

			return Results.Ok(entity);
		});

		builderAction?.Invoke(builder);

		builder = app.MapPost(route, async (HttpContext httpContext, ApplicationDbContext db, TEntity entity) =>
		{
			var business = httpContext.Items["business"] as Business ?? throw new Exception("business not found");
			if (getBusinessId(entity) != business.Id) return Results.Forbid();
			db.Set<TEntity>().Add(entity);
			await db.SaveChangesAsync();
			return Results.Created($"{route}/{entity.Id}", entity);			
		});

		builderAction?.Invoke(builder);

		builder = app.MapPut($"{route}/{{id}}", async (HttpContext httpContext, ApplicationDbContext db, int id, TEntity entity) =>
		{
			var business = httpContext.Items["business"] as Business ?? throw new Exception("business not found");
			if (id != entity.Id) return Results.BadRequest();

			var existingEntity = await query.Invoke(db, id).AsSplitQuery().SingleOrDefaultAsync();
			if (getBusinessId(entity) != business.Id) return Results.Forbid();

			db.Set<TEntity>().Update(entity);
			await db.SaveChangesAsync();
			return Results.Ok(entity);
		});

		builderAction?.Invoke(builder);

		builder = app.MapDelete($"{route}/{{id}}", async (HttpContext httpContext, ApplicationDbContext db, int id) =>
		{
			var business = httpContext.Items["business"] as Business ?? throw new Exception("business not found");
			var entity = await query.Invoke(db, id).AsSplitQuery().SingleOrDefaultAsync();
			
			if (entity is null) return Results.NotFound();
			if (getBusinessId(entity) != business.Id) return Results.Forbid();

			db.Set<TEntity>().Remove(entity);
			await db.SaveChangesAsync();
			return Results.NoContent();
		});

		builderAction?.Invoke(builder);
	}
}
