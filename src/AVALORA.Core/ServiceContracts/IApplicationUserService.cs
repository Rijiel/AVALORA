using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ApplicationUserDtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AVALORA.Core.ServiceContracts;

public interface IApplicationUserService : IGenericService<ApplicationUser, ApplicationUserAddRequest, ApplicationUserUpdateRequest, ApplicationUserResponse>
{
	/// <summary>
	/// Retrieves a list of role options for selection.
	/// </summary>
	/// <param name="roleToCompare">Optional role to compare and gets selected in the list.</param>
	/// <returns>List of SelectListItem representing available roles.</returns>
	List<SelectListItem> GetRoleOptions(string? roleToCompare);
}

