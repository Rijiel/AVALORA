using AVALORA.Core.Domain.Models;

namespace AVALORA.Core.Dto.ProductImageDtos;

public class ProductImageResponse
{
	public int Id { get; set; }
	public string Path { get; set; } = null!;
	public int ProductId { get; set; }
	public Product Product { get; set; } = null!;
}

