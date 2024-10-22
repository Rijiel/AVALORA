using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.Category;

namespace AVALORA.Core.ServiceContracts;

public interface ICategoryService : IGenericService<Category, CategoryAddRequest, CategoryUpdateRequest, CategoryResponse>
{
}

