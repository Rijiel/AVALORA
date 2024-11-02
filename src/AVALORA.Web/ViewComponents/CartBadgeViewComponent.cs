using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.ViewComponents;

public class CartBadgeViewComponent : ViewComponent
{
	private readonly ICartFacade _cartFacade;

	public CartBadgeViewComponent(ICartFacade cartFacade)
	{
		_cartFacade = cartFacade;
	}

	public async Task<IViewComponentResult> InvokeAsync()
	{
		int count = 0;

		// Remove session if not logged in
		if (User.Identity != null && User.Identity.IsAuthenticated)
		{
			// Initialize count if null
			if (HttpContext.Session.GetInt32(SD.SESSION_CART) == null)
			{
				var items = await _cartFacade.GetCurrentUserCartItemsAsync();
				count = items.Count;
				HttpContext.Session.SetInt32(SD.SESSION_CART, count);
			}
			else
				count = HttpContext.Session.GetInt32(SD.SESSION_CART)!.Value;
		}
		else
			HttpContext.Session.Remove(SD.SESSION_CART);

		return View(count);
	}
}
