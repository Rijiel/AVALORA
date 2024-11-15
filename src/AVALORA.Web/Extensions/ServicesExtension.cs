using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Core.Services;
using AVALORA.Core.Services.FacadeServices;
using AVALORA.Infrastructure.DatabaseContext;
using AVALORA.Infrastructure.DbInitializer;
using AVALORA.Infrastructure.Repositories;
using FoolProof.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartBreadcrumbs.Extensions;

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
		services.AddControllersWithViews(options =>
		{
			options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
		});
		services.AddRazorPages();

		services.AddDbContext<ApplicationDbContext>(options => options
		.UseSqlServer(cfg.GetConnectionString("DefaultConnection")));

		services.AddIdentity<IdentityUser, IdentityRole>(options => options.SignIn.RequireConfirmedAccount = false).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

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

		services.ConfigureApplicationCookie(config =>
		{
			config.LoginPath = "/Identity/Account/Login";
			config.AccessDeniedPath = "/Identity/Account/Login";
		});

		services.AddAuthentication()
			.AddFacebook(options =>
			{
				options.AppId = cfg["Facebook:AppID"]!;
				options.AppSecret = cfg["Facebook:AppSecret"]!;
			}).AddGoogle(options =>
			{
				options.ClientId = cfg["Google:ClientID"]!;
				options.ClientSecret = cfg["Google:ClientSecret"]!;
			});

		services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
		services.AddFoolProof();
		services.AddBreadcrumbs(typeof(Program).Assembly, options =>
		{
			options.TagName = "nav";
			options.TagClasses = "mt-3";
			options.OlClasses = "breadcrumb";
			options.LiClasses = "breadcrumb-item";
			options.ActiveLiClasses = "breadcrumb-item active";
			options.SeparatorElement = "&nbsp;<i class=\"bi bi-chevron-right\"></i>&nbsp;";
		});

		services.Configure<PaypalSettings>(config => cfg.GetSection("Paypal").Bind(config));
		services.AddHttpClient<IPaymentService, PaymentService>(client
			=> client.Timeout = TimeSpan.FromMinutes(2));

		services.AddHttpLogging(options =>
		{
			options.LoggingFields = HttpLoggingFields.RequestProperties | HttpLoggingFields.ResponsePropertiesAndHeaders;
		});

		#region DI
		services.AddScoped<IUnitOfWork, UnitOfWork>();
		services.AddScoped<IServiceUnitOfWork, ServiceUnitOfWork>();
		services.AddScoped<IProductFacade, ProductFacade>();
		services.AddScoped<ICartFacade, CartFacade>();
		services.AddScoped<IOrderFacade, OrderFacade>();
		services.AddScoped<IPagerFacade, PagerFacade>();
		services.AddScoped<IEmailSender, EmailSender>();
		services.AddScoped<IDbInitializer, DbInitializer>();
		#endregion

		return services;
	}
}
