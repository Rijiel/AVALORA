using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.CategoryDtos;

namespace AVALORA.Core.ServiceContracts;

public interface ICategoryService : IGenericService<Category, CategoryAddRequest, CategoryUpdateRequest, CategoryResponse>
{
}

