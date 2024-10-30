using AVALORA.Core.Dto.ProductDtos;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for products and categories, containing product responses and categories with products count.
/// </summary>
public class ProductsCategoriesVM
{
    [Required]
    public IEnumerable<ProductResponse> ProductResponses { get; set; } = null!;

    [Required]
    public IEnumerable<ProductResponse> FilteredProducts { get; set; } = null!;

	[Required]
    public Dictionary<string, int> CategoryCounts { get; set; } = null!; // <categoryName, products count>
}

