using AVALORA.Infrastructure.DbInitializer;
using AVALORA.Web.Extensions;
using AVALORA.Web.Middleware;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((HostBuilderContext context, IServiceProvider services, LoggerConfiguration loggerConfiguration) =>
{
	loggerConfiguration
	.ReadFrom.Configuration(context.Configuration)
	.ReadFrom.Services(services);
});

builder.Services.ConfigureServices(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
	app.UseExceptionHandler("/Home/Error");
	app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseHttpLogging();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();
app.UseSession();
SeedDB();
app.MapControllers();
app.MapRazorPages();

//if (app.Environment.IsProduction() || app.Environment.IsStaging() || app.Environment.IsDevelopment())
//{
//	app.UseDataSeedMiddleware();
//}

//app.UseDataSeedMiddleware();

app.Run();

void SeedDB()
{
	using (var scope = app.Services.CreateScope())
	{
		var dbInitializer = scope.ServiceProvider.GetRequiredService<IDbInitializer>();
		dbInitializer.InitializeAsync();
	}
}
