using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class CategoryService : GenericService<Category, CategoryAddRequest, CategoryUpdateRequest, CategoryResponse>, ICategoryService
{
    public CategoryService(ICategoryRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : base(repository, mapper, unitOfWork)
    {        
    }
}

