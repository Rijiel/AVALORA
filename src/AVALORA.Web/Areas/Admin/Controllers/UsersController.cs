using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.ApplicationUserDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Web.Areas.User.Controllers;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

namespace AVALORA.Web.Areas.Admin.Controllers;

[Area(nameof(Role.Admin))]
[Authorize(Roles = nameof(Role.Admin))]
[Route("[controller]/[action]")]
public class UsersController : BaseController<UsersController>
{
	private readonly UserManager<IdentityUser> _userManager;

	public UsersController(UserManager<IdentityUser> userManager)
	{
		_userManager = userManager;
	}

	[Breadcrumb("Users", FromController = typeof(HomeController), FromAction = nameof(HomeController.Index), AreaName = nameof(Role.Admin))]
	public IActionResult Index() => View();

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(string? id)
	{
		var applicationUserResponse = await ServiceUnitOfWork.ApplicationUserService.GetByIdAsync(id);
		if (applicationUserResponse == null)
		{
			Logger.LogError($"User with id {id} not found.");
			return NotFound("User not found!");
		}

		var userRole = await UserHelper.GetUserRoleAsync(id, _userManager);
		applicationUserResponse.Role = userRole ?? string.Empty;

		var applicationUserVM = Mapper.Map<ApplicationUserRoleVM>(applicationUserResponse);
		applicationUserVM.Roles = ServiceUnitOfWork.ApplicationUserService.GetRoleOptions(applicationUserResponse.Role);

		// Setup breadcrumb
		var breadCrumbNode = new MvcBreadcrumbNode(nameof(Index), "Users", "Users", areaName: Role.Admin.ToString());
		var breadCrumbNode1 = new MvcBreadcrumbNode(nameof(Edit), "Users", "Edit", areaName: Role.Admin.ToString())
		{
			OverwriteTitleOnExactMatch = true,
			Parent = breadCrumbNode
		};
		var breadCrumbNode2 = new MvcBreadcrumbNode(nameof(Edit), "Users", id?.ToString(), areaName: Role.Admin.ToString())
		{
			OverwriteTitleOnExactMatch = true,
			Parent = breadCrumbNode1
		};
		ViewData["BreadcrumbNode"] = breadCrumbNode2;

		return View(applicationUserVM);
	}

	[HttpPost]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(ApplicationUserRoleVM applicationUserRoleVM)
	{
		if (ModelState.IsValid)
		{
			IdentityUser? identityUser = await _userManager.FindByIdAsync(applicationUserRoleVM.Id);
			if (identityUser == null)
			{
				Logger.LogError($"User with id {applicationUserRoleVM.Id} not found.");
				return NotFound("User not found!");
			}

			// Different input role: Remove current role and apply input role
			string? oldRole = await UserHelper.GetUserRoleAsync(applicationUserRoleVM.Id, _userManager);
			if (oldRole != applicationUserRoleVM.Role)
			{
				if (!String.IsNullOrEmpty(oldRole))
					await _userManager.RemoveFromRoleAsync(identityUser, oldRole);

				await _userManager.AddToRoleAsync(identityUser, applicationUserRoleVM.Role);
				SuccessMessage = "Role updated successfully.";
			}

			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. User not updated.");
		return View(applicationUserRoleVM);
	}

	public IActionResult Create() => RedirectToPage("/Account/Register", new { area = "Identity" });

	#region API CALLS
	public async Task<IActionResult> GetAll()
	{
		List<ApplicationUserResponse> applicationUserResponses = await ServiceUnitOfWork.ApplicationUserService.GetAllAsync();

		// Initialize user roles
		foreach (var user in applicationUserResponses)
		{
			string? userRole = await UserHelper.GetUserRoleAsync(user.Id, _userManager);
			user.Role = userRole ?? string.Empty;
		}

		return Json(new { data = applicationUserResponses });
	}

	[HttpPost]
	public async Task<IActionResult> LockUnlock(string? id)
	{
		if (!String.IsNullOrEmpty(id))
		{
			IdentityUser? identityUser = await _userManager.FindByIdAsync(id);
			if (identityUser != null)
			{
				// Unlock user if currently locked
				if (await _userManager.IsLockedOutAsync(identityUser))
				{
					await _userManager.SetLockoutEndDateAsync(identityUser, DateTime.Now);

					Logger.LogInformation("User with id " + id + " has been unlocked.");
					SuccessMessage = "User  has been unlocked.";
				}
				// Lock user if currently unlocked
				else
				{
					await _userManager.SetLockoutEndDateAsync(identityUser, DateTime.Now.AddDays(7));

					Logger.LogInformation("User with id " + id + " has been locked.");
					SuccessMessage = "User has been locked.";
				}

				return Json(new { success = true });
			}
		}

		Logger.LogWarning("An error occurred while locking/unlocking user with id " + id + ".");
		ErrorMessage = "An error occurred while locking/unlocking user.";
		return Json(new { success = false });
	}
	#endregion
}
