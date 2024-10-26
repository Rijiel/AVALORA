using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace AVALORA.Core.Helpers;

public static class UserHelper
{
	/// <summary>
	/// Retrieves the current user's ID from the HTTP context.
	/// </summary>
	/// <param name="accessor">The IHttpContextAccessor instance.</param>
	/// <returns>The current user's ID, or null if not authenticated.</returns>
	public static string? GetCurrentUserId(IHttpContextAccessor accessor)
	{
		if (accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
		{
			if (accessor.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
			{
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
				if (userId != null)
					return userId;
			}
		}

		return null;
	}

	/// <summary>
	/// Retrieves the IdentityUser instance associated with the current HTTP context.
	/// </summary>
	/// <param name="accessor">The IHttpContextAccessor instance used to access the current HTTP context.</param>
	/// <param name="userManager">The UserManager instance used to manage IdentityUser instances.</param>
	/// <returns>A Task that represents the asynchronous operation, containing the IdentityUser instance if found, or null otherwise.</returns>
	public static async Task<IdentityUser?> GetIdentityUserAsync(IHttpContextAccessor accessor, UserManager<IdentityUser> userManager, CancellationToken cancellationToken = default)
	{
		string userId = GetCurrentUserId(accessor)!;

		cancellationToken.ThrowIfCancellationRequested();
		return await userManager.FindByIdAsync(userId);
	}

	/// <summary>
	/// Retrieves the current user's role from the HTTP context.
	/// </summary>
	/// <param name="accessor">The IHttpContextAccessor instance.</param>
	/// <returns>The current user's role, or null if not authenticated or if the user does not have a role.</returns>
	public static string? GetUserRole(IHttpContextAccessor accessor)
	{
		if (accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false)
		{
			if (accessor.HttpContext.User.Identity is ClaimsIdentity claimsIdentity)
			{
				var roleClaim = claimsIdentity.FindFirst(ClaimTypes.Role)?.Value;
				if (roleClaim != null)
					return roleClaim;
			}
		}

		return null;
	}

	/// <summary>
	/// Retrieves the role associated with the specified user ID.
	/// </summary>
	/// <param name="userId">The ID of the user to retrieve the role for.</param>
	/// <param name="userManager">The UserManager instance used to manage IdentityUser instances.</param>
	/// <returns>A Task that represents the asynchronous operation, containing the user's role if found, or null otherwise.</returns>
	public static async Task<string?> GetUserRoleAsync(string? userId, UserManager<IdentityUser> userManager, CancellationToken cancellationToken = default)
	{
		if (userId != null)
		{
			var user = await userManager.FindByIdAsync(userId);

			if (user != null)
			{
				var userRoles = await userManager.GetRolesAsync(user);
				return userRoles.FirstOrDefault();
			}
		}

		cancellationToken.ThrowIfCancellationRequested();
		return null;
	}
}

