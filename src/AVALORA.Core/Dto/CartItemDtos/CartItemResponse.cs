using AVALORA.Core.Domain.Models;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Dto.CartItemDtos;

public class CartItemResponse
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;
    public string ApplicationUserId { get; set; } = null!;
    public int Count { get; set; }
    public Color Color { get; set; }
}