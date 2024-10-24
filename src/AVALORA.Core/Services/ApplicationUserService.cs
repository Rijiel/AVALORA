using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ApplicationUserDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class ApplicationUserService : GenericService<ApplicationUser, ApplicationUserAddRequest, ApplicationUserUpdateRequest, ApplicationUserResponse>, IApplicationUserService
{
	public ApplicationUserService(IApplicationUserRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : base(repository, mapper, unitOfWork)
    {
	}
}

