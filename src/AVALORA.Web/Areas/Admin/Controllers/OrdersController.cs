using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.Admin.Controllers;

[Area(nameof(Role.Admin))]
[Route("[controller]/[action]")]
public class OrdersController : BaseController<OrdersController>
{
	private readonly IHttpContextAccessor _contextAccessor;

	public OrdersController(IHttpContextAccessor contextAccessor)
    {
		_contextAccessor = contextAccessor;
	}

    public IActionResult Index()
	{
		return View();
	}

	#region API CALLS
	public async Task<IActionResult> GetAll(string? status)
	{
		List<OrderHeaderResponse> orderHeaderResponseList = await ServiceUnitOfWork.OrderHeaderService.GetAllAsync();
		
		// Show user carts if not admin
		var userId = UserHelper.GetCurrentUserId(_contextAccessor);
		if (!User.IsInRole(Role.Admin.ToString()))
			orderHeaderResponseList = orderHeaderResponseList.Where(o => o.ApplicationUserId == userId).ToList();

		// Filter orders by status (if provided)
		orderHeaderResponseList = (status) switch
		{
			nameof(OrderStatus.Processing) => orderHeaderResponseList.Where(o => o.OrderStatus == OrderStatus.Processing).ToList(),			
			nameof(OrderStatus.Shipped) => orderHeaderResponseList.Where(o => o.OrderStatus == OrderStatus.Shipped).ToList(),
			nameof(OrderStatus.Approved) => orderHeaderResponseList.Where(o => o.OrderStatus == OrderStatus.Approved).ToList(),
			nameof(PaymentStatus.Pending) => orderHeaderResponseList
				.Where(o => o.PaymentStatus == PaymentStatus.Pending || o.PaymentStatus == PaymentStatus.DelayedPayment).ToList(),
			_ => orderHeaderResponseList
		};

		// Use order status description instead of int type order status
		orderHeaderResponseList.ForEach(o => o.OrderStatusDescription = o.OrderStatus.ToString());

		// Populate order summary view model
		List<OrderVM> orderVMs = [];

		foreach (var orderHeaderResponse in orderHeaderResponseList)
		{
			var orderVM = new OrderVM()
			{
				OrderHeaderResponse = orderHeaderResponse,
				OrderSummaryResponse = await ServiceUnitOfWork.OrderSummaryService.GetAsync(o => o.OrderHeaderId == orderHeaderResponse.Id)
			};

			orderVMs.Add(orderVM);
		}

		return Json(new { data = orderVMs });
	}
	#endregion
}
