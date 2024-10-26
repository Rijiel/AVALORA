using AVALORA.Core.Dto.CategoryDtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for managing categories, containing add request and response collection.
/// </summary>
public class CategoriesVM
{
    [Required]
    public CategoryAddRequest CategoryAddRequest { get; set; } = null!;

    [ValidateNever]
    public IEnumerable<CategoryResponse> CategoryResponses { get; set; } = null!;
}

