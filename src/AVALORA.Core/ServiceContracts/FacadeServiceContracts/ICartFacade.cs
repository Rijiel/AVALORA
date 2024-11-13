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
	/// <param name="showMessage">A flag indicating whether to show a success message.</param>
	/// <returns>A task representing the asynchronous operation. 
	/// Returns a TempData message for successful operation.</returns>
	Task UpdateCartItemQuantityAsync(int? cartItemId, int quantity, Controller controller, bool showMessage);

	/// <summary>
	/// Retrieves the cart items of the current user asynchronously.
	/// </summary>
	/// <param name="includeImages">Optional parameter to include images in the response. Defaults to false.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by 
	/// other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation, containing a list of CartItemResponse objects.</returns>
	Task<List<CartItemResponse>> GetCurrentUserCartItemsAsync(bool includeImages = false, 
        CancellationToken cancellationToken = default);

	/// <summary>
	/// Clears the cart items of the current user asynchronously.
	/// </summary>
	/// <param name="controller">The controller instance to send the TempData.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects 
	/// or threads to receive notice of cancellation. Defaults to default.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task ClearCartItemsAsync(Controller controller, CancellationToken cancellationToken = default);

	/// <summary>
	/// Updates the cart session count asynchronously.
	/// </summary>
	/// <param name="controller">The controller instance to get and send the session count.</param>
	/// <param name="quantity">The added/subtracted quantity of the cart item (0 clears the cart).</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	void UpdateCartSessionCount(Controller controller, int quantity);
}

