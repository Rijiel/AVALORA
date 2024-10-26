using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for adding to cart, containing the request and product details.
/// </summary>
public class CartItemAddRequestVM
{
    [Required]
    public CartItemAddRequest CartItemAddRequest { get; set; } = null!;

    [ValidateNever]
    public ProductResponse ProductResponse { get; set; } = null!;
}

