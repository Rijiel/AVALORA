﻿using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Core.Services;
using AVALORA.Core.Services.FacadeServices;
using AVALORA.Infrastructure.DatabaseContext;
using AVALORA.Infrastructure.Repositories;
using FoolProof.Core;
using Microsoft.AspNetCore.Identity;
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

		services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

		services.AddSession(options =>
		{
			options.IdleTimeout = TimeSpan.FromMinutes(30);
			options.Cookie.IsEssential = true;
			options.Cookie.HttpOnly = true; // Make the cookie accessible only via HTTP
		});

		services.Configure<IdentityOptions>(options =>
		{
			options.Password.RequireDigit = false;
			options.Password.RequireLowercase = false;
			options.Password.RequireNonAlphanumeric = false;
			options.Password.RequireUppercase = false;
			options.Password.RequiredLength = 4;

			options.SignIn.RequireConfirmedAccount = false;
			options.SignIn.RequireConfirmedEmail = false;

			options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(7);
			options.Lockout.MaxFailedAccessAttempts = 3;
		});

		services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
		services.AddFoolProof();

		services.Configure<PaypalSettings>(config => cfg.GetSection("Paypal").Bind(config));
		services.AddHttpClient<IPaymentService, PaymentService>(client 
			=> client.Timeout = TimeSpan.FromMinutes(2));

		#region DI
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
		services.AddScoped<IProductFacade, ProductFacade>();
		services.AddScoped<ICartFacade, CartFacade>();
		services.AddScoped<IOrderFacade, OrderFacade>();
		#endregion

		return services;
	}
}
