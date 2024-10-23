using AVALORA.Core.Dto.Category;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AVALORA.Core.Domain.Models.ViewModels;

public class CategoriesVM
{
    public CategoryAddRequest CategoryAddRequest { get; set; } = null!;

    [ValidateNever]
    public ICollection<CategoryResponse> CategoryResponses { get; set; } = null!;
}

