using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using AVALORA.Web.Extensions;
using AVALORA.Web.Filters.ActionFilters;
using AVALORA.Web.Filters.ResultFIlters;
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

	[BindProperty]
	public CheckoutVM CheckoutVM { get; set; } = null!;

	public CartController(ICartFacade cartFacade, IOrderFacade orderFacade, IHttpContextAccessor contextAccessor)
	{
		_cartFacade = cartFacade;
		_orderFacade = orderFacade;
		_contextAccessor = contextAccessor;
	}

	// GET: /Cart/Index
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		List<CartItemResponse> cartItemResponses = await _cartFacade.GetCurrentUserCartItemsAsync(true, cancellationToken);

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

	// GET: /Cart/Subtract/{id}
	[Route("{id?}")]
	public async Task<IActionResult> Subtract(int? id)
	{
		await _cartFacade.UpdateCartItemQuantityAsync(id, -1, this, false);

		return RedirectToAction(nameof(Index));
	}

	// GET: /Cart/Add/{id}
	[Route("{id?}")]
	public async Task<IActionResult> Add(int? id)
	{
		await _cartFacade.UpdateCartItemQuantityAsync(id, +1, this, false);

		return RedirectToAction(nameof(Index));
	}

	// GET: /Cart/Remove/{id}
	[Route("{id?}")]
	public async Task<IActionResult> Remove(int? id)
	{
		await ServiceUnitOfWork.CartItemService.RemoveAsync(id);

		SuccessMessage = "Product removed from cart.";

		_cartFacade.UpdateCartSessionCount(this, -1);

		return RedirectToAction(nameof(Index));
	}

	// GET: /Cart/Checkout
	[HttpGet]
	public async Task<IActionResult> Checkout(CancellationToken cancellationToken)
	{
		// Only allow checkout if cart is not empty
		if (_cartFacade.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken).Result.Count == 0)
		{
			ErrorMessage = "Cart is empty.";
			return RedirectToAction(nameof(Index));
		}

		List<CartItemResponse> cartItemResponses = await _cartFacade
			.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);

		// Initialize the total price for each cart item
		foreach (var item in cartItemResponses)
			item.GetTotalPrice();

		OrderHeaderAddRequest orderHeaderAddRequest = await _orderFacade.CreateOrderHeaderAsync(cancellationToken);

		// Populate the checkout view model with orderheader, cart items and total price
		CheckoutVM = Mapper.Map<CheckoutVM>(orderHeaderAddRequest);
		CheckoutVM.CartItemResponses = cartItemResponses;
		CheckoutVM.TotalPrice = ServiceUnitOfWork.CartItemService.GetTotalPrice(cartItemResponses);

		return View(CheckoutVM);
	}

	// POST: /Cart/Checkout
	[HttpPost]
	[ActionName(nameof(Checkout))]
	[TypeFilter(typeof(ValidationActionFilter))]
	[TypeFilter(typeof(CartControllerResultFilter))]
	public async Task<IActionResult> CheckoutPost(CancellationToken cancellationToken)
	{
		OrderHeaderAddRequest orderHeaderAddRequest = Mapper.Map<OrderHeaderAddRequest>(CheckoutVM);
		OrderSummaryAddRequest orderSummaryAddRequest = Mapper.Map<OrderSummaryAddRequest>(CheckoutVM);

		OrderHeaderResponse orderHeaderResponse = await _orderFacade
			.PlaceOrderAsync(orderHeaderAddRequest, orderSummaryAddRequest, this, cancellationToken);

		// Redirect to payment gateway
		TempData[SD.TEMPDATA_CLEARCART] = true;

		return RedirectToAction("Index", "Payment", new { id = orderHeaderResponse.Id });
	}
}
