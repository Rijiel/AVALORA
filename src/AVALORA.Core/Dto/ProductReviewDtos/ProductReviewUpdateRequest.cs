using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.ProductReviewDtos;

public class ProductReviewUpdateRequest
{
	public int Id { get; set; }

	[StringLength(200)]
	public string? Comment { get; set; }

	[Required]
	public DateTime DatePosted { get; set; }

	[Required]
	public int ProductId { get; set; }

	[Required]
	[Range(1, 5, ErrorMessage = "Please select a rating")]
	public int Rating { get; set; }
}

