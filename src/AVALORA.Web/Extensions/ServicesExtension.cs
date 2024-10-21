namespace AVALORA.Web.Extensions;

public static class ServicesExtension
{
    /// <summary>
    /// Configure startup <see cref="Program"/> services.
    /// </summary>
    /// <param name="services">Collection of services to configure.</param>
    /// <param name="config">Configuration provider.</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();

        return services;
    }
}
