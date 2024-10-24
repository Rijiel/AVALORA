using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.CartItemDtos;

namespace AVALORA.Core.ServiceContracts;

public interface ICartItemService : IGenericService<CartItem, CartItemAddRequest, CartItemUpdateRequest, CartItemResponse>
{
    /// <summary>
    /// Calculates the total price of all given cart items based on the product prices and quantities.
    /// </summary>
    /// <param name="cartItemResponses">The collection of cart item responses.</param>
    /// <returns>The total price of the cart items.</returns>
    double GetTotalPrice(IEnumerable<CartItemResponse>? cartItemResponses);
}

