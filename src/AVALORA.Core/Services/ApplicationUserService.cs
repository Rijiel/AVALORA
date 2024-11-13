using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ApplicationUserDtos;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AVALORA.Core.Services;

public class ApplicationUserService : GenericService<ApplicationUser, ApplicationUserAddRequest, ApplicationUserUpdateRequest, 
	ApplicationUserResponse>, IApplicationUserService
{
	private readonly RoleManager<IdentityRole> _roleManager;

	public ApplicationUserService(IApplicationUserRepository repository, IMapper mapper, IUnitOfWork unitOfWork, 
        RoleManager<IdentityRole> roleManager) : base(repository, mapper, unitOfWork)
    {
		_roleManager = roleManager;
	}

	public List<SelectListItem> GetRoleOptions(string? roleToCompare)
	{
		var roles = _roleManager.Roles.Select(r => new SelectListItem() { Text = r.Name, Value = r.Name }).ToList();

		foreach (var role in roles)
		{
			if (roleToCompare == role.Text)
				role.Selected = true;
		}

		return roles;
	}
}

