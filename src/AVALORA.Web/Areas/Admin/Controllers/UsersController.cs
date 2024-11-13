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

	// GET: /Users/Index
	[Breadcrumb("Users", FromController = typeof(HomeController), FromAction = nameof(HomeController.Index), 
        AreaName = nameof(Role.Admin))]
	public IActionResult Index() => View();

	// GET: /Users/Edit/{id}
	[HttpGet("{id?}")]
	public async Task<IActionResult> Edit(string? id, CancellationToken cancellationToken)
	{
		// Get user to edit
		var applicationUserResponse = await ServiceUnitOfWork.ApplicationUserService
            .GetByIdAsync(id, cancellationToken: cancellationToken);

		if (applicationUserResponse == null)
		{
			Logger.LogError("User with id {userId} not found.", id);
			return NotFound("User not found!");
		}

		var userRole = await UserHelper.GetUserRoleAsync(id, _userManager, cancellationToken);
		applicationUserResponse.Role = userRole ?? string.Empty;

		// Initialize user roles for dropdown
		var applicationUserVM = Mapper.Map<ApplicationUserRoleVM>(applicationUserResponse);
		applicationUserVM.Roles = ServiceUnitOfWork.ApplicationUserService.GetRoleOptions(applicationUserResponse.Role);

        ServiceUnitOfWork.BreadcrumbService.SetCustomNodes(this, "Users", 
            controllerActions: [nameof(Index), nameof(Edit), nameof(Edit)], titles: ["Users", "Edit", id.ToString()!]);

		return View(applicationUserVM);
	}

	// POST: /Users/Edit/{id}
	[HttpPost("{id?}")]
	public async Task<IActionResult> Edit(ApplicationUserRoleVM applicationUserRoleVM, 
        CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			// Get user to edit
			IdentityUser? identityUser = await _userManager.FindByIdAsync(applicationUserRoleVM.Id);
			if (identityUser == null)
			{
				Logger.LogWarning("User with id {userId} not found.", applicationUserRoleVM.Id);
				return NotFound("User not found!");
			}

			// Different input role: Remove current role and apply input role
			string? oldRole = await UserHelper.GetUserRoleAsync(applicationUserRoleVM.Id, 
                _userManager, cancellationToken);

			if (oldRole != applicationUserRoleVM.Role)
			{
				if (!String.IsNullOrEmpty(oldRole))
					await _userManager.RemoveFromRoleAsync(identityUser, oldRole);

				await _userManager.AddToRoleAsync(identityUser, applicationUserRoleVM.Role);

				SuccessMessage = "Role updated successfully.";
                Logger.LogInformation("User with id {userId} role updated to {userRole} successfully.", 
                    applicationUserRoleVM.Id, applicationUserRoleVM.Role);
			}

			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. User not updated. Request details: {updateRequest}", 
            nameof(applicationUserRoleVM));

		// If we got this far, something failed, redisplay form
		return View(applicationUserRoleVM);
	}

	public IActionResult Create() => RedirectToPage("/Account/Register", new { area = "Identity" });

	#region API CALLS
	// GET: /Users/GetAll
	public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
	{
		List<ApplicationUserResponse> applicationUserResponses = await ServiceUnitOfWork.ApplicationUserService
			.GetAllAsync(cancellationToken: cancellationToken);

		// Initialize user roles
		foreach (var user in applicationUserResponses)
		{
			string? userRole = await UserHelper.GetUserRoleAsync(user.Id, _userManager, cancellationToken);
			user.Role = userRole ?? string.Empty;
		}

		return Json(new { data = applicationUserResponses });
	}

	// POST: /Users/LockUnlock?id={id}
	[HttpPost]
	public async Task<IActionResult> LockUnlock(string? id)
	{
		if (!String.IsNullOrEmpty(id))
		{
			// Get user to lock/unlock
			IdentityUser? identityUser = await _userManager.FindByIdAsync(id);
			if (identityUser != null)
			{
				// Unlock user if currently locked
				if (await _userManager.IsLockedOutAsync(identityUser))
				{
					await _userManager.SetLockoutEndDateAsync(identityUser, DateTime.Now);

					Logger.LogInformation("User with id {userId} has been unlocked.", id);
					SuccessMessage = "User  has been unlocked.";
				}
				// Lock user if currently unlocked
				else
				{
					await _userManager.SetLockoutEndDateAsync(identityUser, DateTime.Now.AddDays(7));

					Logger.LogInformation("User with id {userId} has been locked.", id);
					SuccessMessage = "User has been locked.";
				}

				return Json(new { success = true });
			}
		}

		Logger.LogWarning("An error occurred while locking/unlocking user with id {userId}", id);
		ErrorMessage = "An error occurred while locking/unlocking user.";

		return Json(new { success = false });
	}
	#endregion
}
