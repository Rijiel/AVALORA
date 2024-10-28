using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.ProductReviewDtos;

public class ProductReviewAddRequest
{
	[StringLength(200)]
	public string? Comment { get; set; }

	[Required]
	public DateTime DatePosted { get; set; }

	[Required]
	public int ProductId { get; set; }

	[Required]
	[Range(1, 5)]
	public int Rating { get; set; }

	[Required]
	[StringLength(50)]
	public string UserName { get; set; } = null!;
}

