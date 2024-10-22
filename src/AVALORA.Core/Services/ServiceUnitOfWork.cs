using AutoMapper;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class ServiceUnitOfWork : IServiceUnitOfWork
{
    public ICategoryService CategoryService { get; private set; }

    public ServiceUnitOfWork(IUnitOfWork unitOfWork, IMapper mapper)
    {
        CategoryService = new CategoryService(unitOfWork.Categories, mapper, unitOfWork);
    }
}

