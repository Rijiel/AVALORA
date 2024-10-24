using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Core.ServiceContracts.FacadeServiceContracts;

public interface ICartFacade
{
	/// <summary>
	/// Adds a cart item to the database asynchronously.
	/// </summary>
	/// <param name="cartItemAddRequest">The request containing the cart item details.</param>
	/// <param name="controller">The controller instance to send the TempData.</param>
	/// <returns>A task representing the asynchronous operation. 
	/// Returns a TempData message for sucessful operation.</returns>
	Task AddToCartAsync(CartItemAddRequest? cartItemAddRequest, Controller controller);


	/// <summary>
	/// Updates the quantity of a cart item in the database asynchronously.
	/// </summary>
	/// <param name="cartItemId">The ID of the cart item to update.</param>
	/// <param name="quantity">The new quantity of the cart item.</param>
	/// <param name="controller">The controller instance to send the TempData.</param>
	/// <returns>A task representing the asynchronous operation. 
	/// Returns a TempData message for successful operation.</returns>
	Task UpdateCartItemQuantityAsync(int? cartItemId, int quantity, Controller controller);
}

