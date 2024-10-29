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
	[StringLength(200)]
	public string Description { get; set; } = null!;

	[Required]
	[Range(1, 5000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	[LessThan(nameof(Price), ErrorMessage = "{0} must be less than the {1}.")]
	[DataType(DataType.Currency)]
	[DisplayName("List Price")]
	public double ListPrice { get; set; }

	[Required]
	[Range(1, 5000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	[DataType(DataType.Currency)]
	public double Price { get; set; }

	[Required(ErrorMessage = "Please select a category")]
	[DisplayName("Category")]
	public int CategoryId { get; set; }

	[Range(0, 5)]
	[Column(TypeName = "decimal(18, 2)")]
	[DisplayName("Total Rating")]
	public decimal TotalRating { get; set; }

	[Required]
	public List<Color> Colors { get; set; } = [Color.None];

	[DisplayName("Image Files")]
	public IEnumerable<IFormFile>? ImageFiles { get; set; }

	[DisplayName("Product Images")]
	public ICollection<ProductImage>? ProductImages { get; set; }
}

