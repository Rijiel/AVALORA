using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.CartItemDtos;

public class CartItemUpdateRequest
{
    public int Id { get; set; }

    [Required]
    [Range(1, 20, ErrorMessage = "You can only order maximum 20 items at a time")]
    public int Count { get; set; }
}

