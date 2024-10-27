using AVALORA.Core.Domain.Models;

namespace AVALORA.Core.Dto.ProductReviewDtos;

public class ProductReviewResponse
{
	public int Id { get; set; }
	public string? Comment { get; set; }
	public DateTime DatePosted { get; set; }
	public int ProductId { get; set; }
	public Product? Product { get; set; }
	public int Rating { get; set; }
}

