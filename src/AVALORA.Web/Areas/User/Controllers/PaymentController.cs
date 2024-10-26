using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Route("[controller]/[action]")]
public class PaymentController : BaseController<PaymentController>
{
	private readonly ICartFacade _cartFacade;

	public PaymentController(ICartFacade cartFacade)
	{
		_cartFacade = cartFacade;
	}

	public async Task<IActionResult> Index(int? id, CancellationToken cancellationToken)
	{
		if (id != null)
			HttpContext.Session.SetInt32(SD.TEMPDATA_ORDERID, (int)id);

		OrderSummaryResponse? orderSummaryResponse = await ServiceUnitOfWork.OrderSummaryService
			.GetAsync(o => o.OrderHeaderId == id);

		if (orderSummaryResponse == null)
		{
			Logger.LogWarning("Order not found");
			return NotFound("Order not found");
		}

		// Empty all cart items
		await _cartFacade.ClearCartItemsAsync(this, cancellationToken);

		// TODO: Add payment gateway integration
		return RedirectToAction(nameof(OrderConfirmation), new { id });
	}

	public async Task<IActionResult> OrderConfirmation(int? id, CancellationToken cancellationToken)
	{
		var orderHeaderId = id ?? HttpContext.Session.GetInt32(SD.TEMPDATA_ORDERID);
		OrderHeaderResponse? orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderSevice.GetByIdAsync(orderHeaderId);
		if (orderHeaderResponse == null)
		{
			Logger.LogWarning("Order not found");
			return NotFound("Order not found");
		}

		await ServiceUnitOfWork.OrderHeaderSevice.UpdateOrderStatusAsync(orderHeaderId, OrderStatus.Approved, PaymentStatus.Approved, cancellationToken);

		return View(orderHeaderResponse);
	}
}
