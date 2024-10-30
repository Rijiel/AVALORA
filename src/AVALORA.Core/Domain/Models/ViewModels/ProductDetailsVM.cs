using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for product details, containing cart item add request view model 
/// and product review view model.
/// </summary>
public class ProductDetailsVM
{
    [Required]
    public CartItemAddRequestVM CartItemAddRequestVM { get; set; } = null!;

	[Required]
    public ProductReviewVM ProductReviewVM { get; set; } = null!;
}

