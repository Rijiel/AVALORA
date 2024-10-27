using AVALORA.Core.Dto.ProductReviewDtos;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

public class ProductReviewVM
{
	[Required]
	public ProductReviewAddRequest ProductReviewAddRequest { get; set; } = null!;

	public IEnumerable<ProductReviewResponse>? ProductReviewResponses { get; set; }
}

