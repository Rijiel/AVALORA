using AVALORA.Core.Helpers;
using AVALORA.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace AVALORA.IntegrationTests;

internal class MockWebApplicationFactory : WebApplicationFactory<Program>
{
	protected override void ConfigureWebHost(IWebHostBuilder builder)
	{
		base.ConfigureWebHost(builder);

		builder.UseEnvironment(SD.TEST_ENVIRONMENT);

		builder.ConfigureServices(services =>
		{
			// Remove the default DbContext
			services.RemoveAll(typeof(DbContextOptions<ApplicationDbContext>));

			// Add inmemory database
			services.AddDbContext<ApplicationDbContext>(option =>
			{
				option.UseInMemoryDatabase("InMemoryDbForTesting");
			});

			// Disable Antiforgery
			services.PostConfigure<MvcOptions>(options =>
			{
				options.Filters.Remove(new AutoValidateAntiforgeryTokenAttribute());
			});

			// Override Authentication
			services.AddTransient<IAuthenticationSchemeProvider, MockSchemeProvider>();
		});
	}
}

