using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models;

public class ProductImage
{
	public int Id { get; set; }

	[Required]
	public string Path { get; set; } = null!;

	[Required]
	public int ProductId { get; set; }

	[ForeignKey(nameof(ProductId))]
	public Product Product { get; set; } = null!;
}

