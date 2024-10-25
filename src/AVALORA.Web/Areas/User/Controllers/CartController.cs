using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Route("[controller]/[action]")]
public class CartController : BaseController<CartController>
{
	private readonly ICartFacade _cartFacade;
	private readonly IHttpContextAccessor _contextAccessor;

	public CartController(ICartFacade cartFacade, IHttpContextAccessor contextAccessor)
    {
		_cartFacade = cartFacade;
		_contextAccessor = contextAccessor;
	}

    public async Task<IActionResult> Index()
	{
		List<CartItemResponse> cartItemResponses = await _cartFacade.GetCurrentUserCartItemsAsync(true);

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
		TempData[SD.TEMPDATA_SUCCESS] = "Product removed from cart.";
		Logger.LogInformation($"Removed cart item: {id}");

		return RedirectToAction(nameof(Index));
	}
}
