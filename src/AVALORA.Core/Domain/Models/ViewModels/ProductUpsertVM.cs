using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using FoolProof.Core;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for inserting and updating products, containing product details and metadata.
/// </summary>
public class ProductUpsertVM
{
	[Required]
	public int Id { get; set; }

	[Required]
	[StringLength(50)]
	public string Name { get; set; } = null!;

	[Required]
	[StringLength(200)]
	public string Description { get; set; } = null!;

	[Range(0, 5000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	[LessThan(nameof(Price), ErrorMessage = "{0} must be less than the {1}.")]
	[DataType(DataType.Currency)]
	[DisplayName("List Price")]
	public double? ListPrice { get; set; } = 0;

	[Required]
	[Range(1, 5000, ErrorMessage = "Value for {0} must be between {1} and {2}.")]
	[DataType(DataType.Currency)]
	public double Price { get; set; }

	[Required(ErrorMessage = "Please select a category")]
	[DisplayName("Category")]
	public int CategoryId { get; set; }

	[Required]
	public List<Color> Colors { get; set; } = [Color.None];

	[DisplayName("Image Files")]
	public IEnumerable<IFormFile>? ImageFiles { get; set; }

	[DisplayName("Product Images")]
	public ICollection<ProductImage>? ProductImages { get; set; }

	[ValidateNever]
    public IEnumerable<SelectListItem> Categories { get; set; } = null!;
}
