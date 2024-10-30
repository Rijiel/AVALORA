using AVALORA.Core.Dto.ProductReviewDtos;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for product reviews, containing add request and response collection.
/// </summary>
public class ProductReviewVM
{
	[Required]
	public ProductReviewAddRequest ProductReviewAddRequest { get; set; } = null!;

	public IEnumerable<ProductReviewResponse>? ProductReviewResponses { get; set; }
}

