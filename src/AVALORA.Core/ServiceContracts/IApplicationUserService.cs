using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ApplicationUserDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IApplicationUserService : IGenericService<ApplicationUser, ApplicationUserAddRequest, ApplicationUserUpdateRequest, ApplicationUserResponse>
{
}

