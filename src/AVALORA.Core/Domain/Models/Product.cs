using AVALORA.Core.Enums;
using FoolProof.Core;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVALORA.Core.Domain.Models;

public class Product
{
	public int Id { get; set; }

	[Required]
	[Column(TypeName = "varchar(50)")]
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

	[Required]
	[Range(1, int.MaxValue, ErrorMessage = "Please select a category")]
	public int CategoryId { get; set; }

	[ForeignKey(nameof(CategoryId))]
	public Category Category { get; set; } = null!;

	[Range(0, 5)]
	[Column(TypeName = "decimal(18, 2)")]
	[DisplayName("Total Rating")]
	public decimal TotalRating { get; set; }

	[Required]
	public List<Color> Colors { get; set; } = [Color.None];

	public ICollection<ProductImage>? ProductImages { get; set; }

	public ICollection<ProductReview>? ProductReviews { get; set; }
}

