using AVALORA.Core.Dto.CategoryDtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AVALORA.Core.Domain.Models.ViewModels;

public class CategoriesVM
{
    public CategoryAddRequest CategoryAddRequest { get; set; } = null!;

    [ValidateNever]
    public IEnumerable<CategoryResponse> CategoryResponses { get; set; } = null!;
}

