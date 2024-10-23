using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.ProductImageDtos;

public class ProductImageAddRequest
{
	[Required]
	public string Path { get; set; } = null!;

	[Required]
	public int ProductId { get; set; }
}

