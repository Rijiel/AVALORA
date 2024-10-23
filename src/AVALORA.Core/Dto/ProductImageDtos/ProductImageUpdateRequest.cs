using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.ProductImageDtos;

public class ProductImageUpdateRequest
{
	[Required]
	public int Id { get; set; }

	[Required]
	public string Path { get; set; } = null!;
}

