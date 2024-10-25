using AVALORA.Core.Dto.CartItemDtos;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

public class CartItemResponsesVM
{
    [Required]
    public IEnumerable<CartItemResponse> CartItemResponses { get; set; } = null!;

    [Required]
	[DataType(DataType.Currency)]
    public double TotalPrice { get; set; }
}

