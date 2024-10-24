using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.CartItemDtos;

public class CartItemAddRequest
{
    [Required]
    public int ProductId { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = null!;

    [Required]
    [Range(1, 20, ErrorMessage = "You can only order maximum 20 items at a time")]
    public int Count { get; set; }
}

