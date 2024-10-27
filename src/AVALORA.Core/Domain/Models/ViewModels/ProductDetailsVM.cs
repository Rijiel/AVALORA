using AVALORA.Core.Dto.ProductReviewDtos;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

public class ProductDetailsVM
{
    [Required]
    public CartItemAddRequestVM CartItemAddRequestVM { get; set; } = null!;

	[Required]
    public ProductReviewVM ProductReviewVM { get; set; } = null!;
}

