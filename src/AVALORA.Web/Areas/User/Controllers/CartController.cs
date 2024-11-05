using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using AVALORA.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Authorize]
[Route("[controller]/[action]")]
public class CartController : BaseController<CartController>
{
	private readonly ICartFacade _cartFacade;
	private readonly IOrderFacade _orderFacade;
	private readonly IHttpContextAccessor _contextAccessor;

	public CartController(ICartFacade cartFacade, IOrderFacade orderFacade, IHttpContextAccessor contextAccessor)
	{
		_cartFacade = cartFacade;
		_orderFacade = orderFacade;
		_contextAccessor = contextAccessor;
	}

	public async Task<IActionResult> Index()
	{
		List<CartItemResponse> cartItemResponses = await _cartFacade.GetCurrentUserCartItemsAsync(true);

		// Initialize the total price for each cart item
		foreach (var item in cartItemResponses)
			item.GetTotalPrice();

		// Create a new CartItemResponsesVM to hold the cart item responses and total price
		var cartItemResponsesVM = new CartItemResponsesVM
		{
			CartItemResponses = cartItemResponses,
			TotalPrice = ServiceUnitOfWork.CartItemService.GetTotalPrice(cartItemResponses)
		};

		return View(cartItemResponsesVM);
	}

	[Route("{id?}")]
	public async Task<IActionResult> Subtract(int? id)
	{
		await _cartFacade.UpdateCartItemQuantityAsync(id, -1, this, false);

		return RedirectToAction(nameof(Index));
	}

	[Route("{id?}")]
	public async Task<IActionResult> Add(int? id)
	{
		await _cartFacade.UpdateCartItemQuantityAsync(id, +1, this, false);

		return RedirectToAction(nameof(Index));
	}

	[Route("{id?}")]
	public async Task<IActionResult> Remove(int? id)
	{
		await ServiceUnitOfWork.CartItemService.RemoveAsync(id);
		SuccessMessage = "Product removed from cart.";
		Logger.LogInformation($"Removed cart item: {id}");

		_cartFacade.UpdateCartSessionCount(this, -1);

		return RedirectToAction(nameof(Index));
	}

	[HttpGet]
	public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
	{
		// Only allow checkout if cart is not empty
		if (_cartFacade.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken).Result.Count == 0)
		{
			ErrorMessage = "Cart is empty.";
			return RedirectToAction(nameof(Index));
		}

		List<CartItemResponse> cartItemResponses = await _cartFacade.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);

		// Initialize the total price for each cart item
		foreach (var item in cartItemResponses)
			item.GetTotalPrice();

		OrderHeaderAddRequest orderHeaderAddRequest = await _orderFacade.CreateOrderHeaderAsync(cancellationToken);

		// Populate the checkout view model with orderheader, cart items and total price
		var checkoutVM = Mapper.Map<CheckoutVM>(orderHeaderAddRequest);
		checkoutVM.CartItemResponses = cartItemResponses;
		checkoutVM.TotalPrice = ServiceUnitOfWork.CartItemService.GetTotalPrice(cartItemResponses);

		return View(checkoutVM);
	}

	[HttpPost]
	public async Task<IActionResult> Checkout(CheckoutVM checkoutVM, CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			OrderHeaderAddRequest orderHeaderAddRequest = Mapper.Map<OrderHeaderAddRequest>(checkoutVM);
			OrderSummaryAddRequest orderSummaryAddRequest = Mapper.Map<OrderSummaryAddRequest>(checkoutVM);

			OrderHeaderResponse orderHeaderResponse = await _orderFacade.PlaceOrderAsync(orderHeaderAddRequest,
				orderSummaryAddRequest, this, cancellationToken);

			// Redirect to payment gateway
			TempData[SD.TEMPDATA_CLEARCART] = true;
			return RedirectToAction("Index", "Payment", new { id = orderHeaderResponse.Id });
		}

		checkoutVM.CartItemResponses = await _cartFacade.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);
		return View(checkoutVM);
	}
}
