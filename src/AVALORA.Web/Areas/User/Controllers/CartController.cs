using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
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
		await _cartFacade.UpdateCartItemQuantityAsync(id, -1, this);

		return RedirectToAction(nameof(Index));
	}

	[Route("{id?}")]
	public async Task<IActionResult> Add(int? id)
	{
		await _cartFacade.UpdateCartItemQuantityAsync(id, +1, this);

		return RedirectToAction(nameof(Index));
	}

	[Route("{id?}")]
	public async Task<IActionResult> Remove(int? id)
	{
		await ServiceUnitOfWork.CartItemService.RemoveAsync(id);
		SuccessMessage = "Product removed from cart.";
		Logger.LogInformation($"Removed cart item: {id}");

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
			return RedirectToAction("Index", "Payment", new { id = orderHeaderResponse.Id });
		}

		checkoutVM.CartItemResponses = await _cartFacade.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);
		return View(checkoutVM);
	}
}
