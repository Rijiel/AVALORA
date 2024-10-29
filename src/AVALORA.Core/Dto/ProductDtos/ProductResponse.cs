using AVALORA.Core.Domain.Models;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Dto.ProductDtos;

public class ProductResponse
{
	public int Id { get; set; }
	public string Name { get; set; } = null!;
	public string Description { get; set; } = null!;
	public double ListPrice { get; set; }
	public double Price { get; set; }
	public int CategoryId { get; set; }
	public Category Category { get; set; } = null!;
	public ICollection<ProductImage>? ProductImages { get; set; }
	public ICollection<ProductReview>? ProductReviews { get; set; }
	public int? ProductImagesCount { get; set; }
	public decimal TotalRating { get; set; }
	public List<Color> Colors { get; set; } = null!;
}

