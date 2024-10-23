﻿using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using AVALORA.Infrastructure.DatabaseContext;
using AVALORA.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AVALORA.Web.Extensions;

public static class ServicesExtension
{
    /// <summary>
    /// Configure startup <see cref="Program"/> services.
    /// </summary>
    /// <param name="services">Collection of services to configure.</param>
    /// <param name="cfg">Configuration provider.</param>
    /// <returns></returns>
    public static IServiceCollection ConfigureServices(this IServiceCollection services, IConfiguration cfg)
    {
        services.AddControllersWithViews();
        services.AddRazorPages();

        services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

        services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

        #region DI
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
        #endregion

        return services;
    }
}
