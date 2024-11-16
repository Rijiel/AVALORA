using AVALORA.Infrastructure.DbInitializer;

namespace AVALORA.Web.Middleware;
// You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
public class DataSeedMiddleware
{
	private readonly RequestDelegate _next;

	public DataSeedMiddleware(RequestDelegate next)
	{
		_next = next;
	}

	public Task Invoke(HttpContext httpContext, IDbInitializer dbInitializer)
	{
		//dbInitializer.InitializeAsync().GetAwaiter().GetResult();

		return _next(httpContext);
	}
}

// Extension method used to add the middleware to the HTTP request pipeline.
public static class DataSeedMiddlewareExtensions
{
	public static IApplicationBuilder UseDataSeedMiddleware(this IApplicationBuilder builder)
	{
		return builder.UseMiddleware<DataSeedMiddleware>();
	}
}
