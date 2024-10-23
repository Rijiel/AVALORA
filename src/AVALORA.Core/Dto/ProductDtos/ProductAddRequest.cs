using AVALORA.Core.Domain.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Microsoft.AspNetCore.Http;

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

	[DisplayName("Image Files")]
	public ICollection<IFormFile>? ImageFiles { get; set; }

	public ICollection<ProductImage>? ProductImages { get; set; }
}

