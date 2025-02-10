This is a set of objects to make it easy to get the current `IdentityUser` in your Blazor apps without additional database queries.

1. Add the [CurrentUserAccessor](https://github.com/adamfoneil/LiteInvoice/blob/master/AuthExtensions/CurrentUserAccessor.cs) to your service collection in startup:
```csharp
builder.Services.AddScoped<CurrentUserAccessor<ApplicationDbContext, ApplicationUser>>();
```
where `ApplicationDbContext` is your db context class, and `ApplicationUser` is your `IdentityUser` type.

2. In `Routes.razor` surround the existing markup with the [CurrentUserProvider](https://github.com/adamfoneil/LiteInvoice/blob/master/AuthExtensions/CurrentUserProvider.razor)
```csharp
@using AuthExtensions
<CurrentUserProvider TDbContext="ApplicationDbContext" TUser="ApplicationUser">
    .... markup omitted for clarity
</CurrentUserProvider>
```
Substitute your proper `TDbContext` and `TUser` types.

3. In components where you need to the current `TUser` add a cascading parameter:
```csharp
[CascadingParameter]
public ApplicationUser? CurrentUser { get; set; }
```
Make it nullable and check for nulls when accessing it, but it will be set to the current logged in user automatically without additional database queries.
