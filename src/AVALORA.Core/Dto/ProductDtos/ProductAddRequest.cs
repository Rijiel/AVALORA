using AVALORA.Core.Domain.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;
using FoolProof.Core;
using System.ComponentModel.DataAnnotations.Schema;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Dto.ProductDtos;

public class ProductAddRequest
{
	[Required]
	[StringLength(50)]
	public string Name { get; set; } = null!;

	[Required]
	[MinLength(50, ErrorMessage = "Description must be at least {1} characters long")]
	public string Description { get; set; } = null!;

	[Range(0, 5000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	[GreaterThan(nameof(Price), PassOnNull = true, ErrorMessage = "Value for {0} must be greater than {1}.")]
	[DataType(DataType.Currency)]
	[DisplayName("List Price")]
	public double? ListPrice { get; set; } = 0;

	[Required]
	[Range(1, 5000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	[DataType(DataType.Currency)]
	public double Price { get; set; }

	[Required(ErrorMessage = "Please select a category")]
	[Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
	[DisplayName("Category")]
	public int CategoryId { get; set; }

	[Range(0, 5)]
	[DisplayName("Total Rating")]
	public decimal TotalRating { get; set; }

	[Required]
	public List<Color> Colors { get; set; } = [Color.None];

	[DisplayName("Image Files")]
	public IEnumerable<IFormFile>? ImageFiles { get; set; }

	[DisplayName("Product Images")]
	public ICollection<ProductImage>? ProductImages { get; set; }
}

