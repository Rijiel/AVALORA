using AVALORA.Core.Domain.Models;
using AVALORA.Core.Enums;
using AVALORA.Infrastructure.DatabaseContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AVALORA.Infrastructure.DbInitializer;

public class DbInitializer : IDbInitializer
{
	private readonly ApplicationDbContext _dbContext;
	private readonly RoleManager<IdentityRole> _roleManager;
	private readonly UserManager<IdentityUser> _userManager;

	public DbInitializer(ApplicationDbContext dbContext, RoleManager<IdentityRole> roleManager,
		UserManager<IdentityUser> userManager)
	{
		_dbContext = dbContext;
		_roleManager = roleManager;
		_userManager = userManager;
	}

	public async Task InitializeAsync()
	{
		// apply migrations
		try
		{
			if (_dbContext.Database.GetPendingMigrations().Count() > 0)
				_dbContext.Database.Migrate();
		}
		catch (Exception)
		{
			throw;
		}

		// initialize roles
		if (!await _roleManager.RoleExistsAsync(Role.Admin.ToString()))
		{
			await _roleManager.CreateAsync(new IdentityRole(Role.Admin.ToString()));
			await _roleManager.CreateAsync(new IdentityRole(Role.User.ToString()));
		}

		if (await _userManager.FindByEmailAsync("admin@avalora.com") == null)
		{
			// create admin role
			var adminUser = new ApplicationUser()
			{
				Name = "Admin",
				UserName = "admin@avalora.com",
				Email = "admin@avalora.com",
				Address = "4068 Olen Thomas Drive",
				EmailConfirmed = true,
				LockoutEnabled = false
			};

			var result = await _userManager.CreateAsync(adminUser, password: "admin123");

			if (result.Succeeded)
				await _userManager.AddToRoleAsync(adminUser, Role.Admin.ToString());
		}		
	}
}

