using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Web.BaseController;
using AVALORA.Web.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Authorize]
[Route("[controller]/[action]")]
public class OrdersController : BaseController<OrdersController>
{
	private readonly IHttpContextAccessor _contextAccessor;

	[BindProperty]
	public OrderVM OrderVM { get; set; } = null!;

	public OrdersController(IHttpContextAccessor contextAccessor)
	{
		_contextAccessor = contextAccessor;
	}

	// GET: /Orders/Index
	[Breadcrumb("Orders", FromController = typeof(HomeController), FromAction = nameof(HomeController.Index),
		AreaName = nameof(Role.User))]
	public IActionResult Index()
	{
		return View();
	}

	// GET: /Orders/Edit/{id}
	[Route("{id?}")]
	public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
	{
		// Get order header to edit
		OrderHeaderResponse? orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderService
			.GetByIdAsync(id, cancellationToken: cancellationToken);

		if (orderHeaderResponse == null)
		{
			Logger.LogWarning("Order {orderId} not found.", id);
			return NotFound("Order not found.");
		}

		OrderSummaryResponse? orderSummaryResponse = await ServiceUnitOfWork.OrderSummaryService
			.GetAsync(o => o.OrderHeaderId == id, includes: nameof(OrderSummaryResponse.OrderSummaryItems));

		// Initialize the total price for each order summary item
		if (orderSummaryResponse != null)
		{
			foreach (var item in orderSummaryResponse.OrderSummaryItems)
				item.GetTotalPrice();
		}

		// Initialize order view model with order header and order summary
		OrderVM = new OrderVM()
		{
			OrderHeaderResponse = orderHeaderResponse,
			OrderSummaryResponse = orderSummaryResponse
		};

		// Receive model state errors from other actions' redirection to this controller action.
		ValidationHelper.AddModelState(this, nameof(OrderHeaderResponse));

		ServiceUnitOfWork.BreadcrumbService.SetCustomNodes(this, "Orders",
			controllerActions: [nameof(Index), nameof(Edit), nameof(Edit)], titles: ["Orders", "Edit", id?.ToString()]);

		return View(OrderVM);
	}

	// POST: /Orders/UpdateDetails
	[Authorize(Roles = nameof(Role.Admin))]
	[HttpPost]
	public async Task<IActionResult> UpdateDetails()
	{
		if (ModelState.IsValid)
		{
			var orderHeaderUpdateRequest = Mapper.Map<OrderHeaderUpdateRequest>(OrderVM.OrderHeaderResponse);

			// Update only necessary fields
			await ServiceUnitOfWork.OrderHeaderService.UpdatePartialAsync(orderHeaderUpdateRequest, propertyNames:
				[
					nameof(OrderHeaderUpdateRequest.Name),
					nameof(OrderHeaderUpdateRequest.PhoneNumber),
					nameof(OrderHeaderUpdateRequest.Email),
					nameof(OrderHeaderUpdateRequest.Address),
					nameof(OrderHeaderUpdateRequest.Carrier),
					nameof(OrderHeaderUpdateRequest.TrackingNumber)
				]);

			SuccessMessage = "Pickup details updated successfully.";

			return RedirectToAction(nameof(Edit), new { id = OrderVM.OrderHeaderResponse.Id });
		}

		Logger.LogWarning("Pickup details update failed. Request details: {updateRequest}", nameof(OrderVM));

		// If we got this far, something failed, redisplay form
		return RedirectToAction(nameof(Edit), new { id = OrderVM.OrderHeaderResponse.Id });
	}

	// GET: /Orders/Pay?id={id}
	public async Task<IActionResult> Pay(int? id, CancellationToken cancellationToken)
	{
		// Check if orderheader user id is current user
		OrderHeaderResponse? orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderService
			.GetByIdAsync(id, cancellationToken: cancellationToken);

		if (orderHeaderResponse == null)
		{
			Logger.LogWarning("Order {orderId} not found.", id);
			return NotFound("Order not found!");
		}

		// Only allow payment for current user's orders
		if (orderHeaderResponse.ApplicationUserId == UserHelper.GetCurrentUserId(_contextAccessor))
			return RedirectToAction("Index", "Payment", new { id });

		ErrorMessage = "Cannot pay at this moment.";
		Logger.LogWarning("Cannot pay at this moment. Order ID: {orderId}", id);

		// If we got this far, something failed, redisplay form
		return RedirectToAction(nameof(Edit), new { id });
	}

	// GET: /Orders/Process?id={id}
	public async Task<IActionResult> Process(int id)
	{
		await ServiceUnitOfWork.OrderHeaderService.UpdateOrderStatusAsync(id, OrderStatus.Processing);
		SuccessMessage = "Order status updated successfully";

		return RedirectToAction(nameof(Edit), new { id });
	}

	// GET: /Orders/Ship
	public async Task<IActionResult> Ship()
	{
		// Use a validator DTO (makes optional/nullable fields required) to check if pickup details are provided
		var orderHeaderValidatorDTO = Mapper.Map<OrderHeaderValidatorDTO>(OrderVM.OrderHeaderResponse);
		if (ValidationHelper.IsModelStateValid(orderHeaderValidatorDTO, this))
		{
			// Update details from input
			var orderHeaderUpdateRequest = Mapper.Map<OrderHeaderUpdateRequest>(OrderVM.OrderHeaderResponse);

			// Update only necessary fields
			await ServiceUnitOfWork.OrderHeaderService.UpdatePartialAsync(orderHeaderUpdateRequest, propertyNames:
				[
					nameof(OrderHeaderUpdateRequest.Name),
					nameof(OrderHeaderUpdateRequest.PhoneNumber),
					nameof(OrderHeaderUpdateRequest.Email),
					nameof(OrderHeaderUpdateRequest.Address),
					nameof(OrderHeaderUpdateRequest.Carrier),
					nameof(OrderHeaderUpdateRequest.TrackingNumber)
				]);

			await ServiceUnitOfWork.OrderHeaderService.UpdateOrderStatusAsync(OrderVM.OrderHeaderResponse.Id,
				OrderStatus.Shipped);

			SuccessMessage = "Order status updated successfully";

			return RedirectToAction(nameof(Edit), new { OrderVM.OrderHeaderResponse.Id });
		}

		ErrorMessage = "Please provide pickup details.";
		return RedirectToAction(nameof(Edit), new { OrderVM.OrderHeaderResponse.Id });
	}

	// GET: /Orders/Cancel?id={id}
	public async Task<IActionResult> Cancel(int? id, [FromServices] IOptions<PaypalSettings> paypal,
		CancellationToken cancellationToken)
	{
		// Issue a refund from PayPal
		OrderHeaderResponse? orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderService
			.GetByIdAsync(id, cancellationToken: cancellationToken);

		if (orderHeaderResponse?.PaymentID != null)
		{
			string url = paypal.Value.SandboxURL + $"/v2/payments/captures/{orderHeaderResponse.PaymentID}/refund";
			string authHeaderValue = "Bearer " + await ServiceUnitOfWork.PaymentService
				.GetPaypalAccessTokenAsync(paypal.Value, cancellationToken);

			var httpContent = new StringContent("", null, "application/json");

			var jsonReponse = await ServiceUnitOfWork.PaymentService
				.SendRequestAsync(url, authHeaderValue, httpContent);

			if (jsonReponse != null)
			{
				// Refund success
				await ServiceUnitOfWork.OrderHeaderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled,
					PaymentStatus.Refunded);

				SuccessMessage = "Order has been cancelled and refunded.";
				Logger.LogInformation("Order has been cancelled and refunded: {paymentId}.",
					orderHeaderResponse.PaymentID);

				return RedirectToAction(nameof(Edit), new { id });
			}
			else
			{
				ErrorMessage = "An error has occurred.";
				Logger.LogWarning("An error has occurred while refunding {paymentId}.", orderHeaderResponse.PaymentID);

				return RedirectToAction(nameof(Edit), new { id });
			}
		}

		// If order has not been paid yet, just cancel order
		await ServiceUnitOfWork.OrderHeaderService.UpdateOrderStatusAsync(id, OrderStatus.Cancelled,
			PaymentStatus.Cancelled);
		SuccessMessage = "Order has been cancelled.";

		return RedirectToAction(nameof(Edit), new { id });
	}

	#region API CALLS
	// GET: /Orders/GetAll
	public async Task<IActionResult> GetAll(string? status, CancellationToken cancellationToken)
	{
		List<OrderHeaderResponse> orderHeaderResponseList = await ServiceUnitOfWork.OrderHeaderService
			.GetAllAsync(cancellationToken: cancellationToken);

		// Show carts only from user if not admin
		var userId = UserHelper.GetCurrentUserId(_contextAccessor);
		if (!User.IsInRole(Role.Admin.ToString()))
			orderHeaderResponseList = orderHeaderResponseList.Where(o => o.ApplicationUserId == userId).ToList();

		// Filter orders by status (if provided)
		orderHeaderResponseList = status switch
		{
			nameof(OrderStatus.Processing) => orderHeaderResponseList
			.Where(o => o.OrderStatus == OrderStatus.Processing).ToList(),

			nameof(OrderStatus.Shipped) => orderHeaderResponseList
			.Where(o => o.OrderStatus == OrderStatus.Shipped).ToList(),

			nameof(OrderStatus.Approved) => orderHeaderResponseList
			.Where(o => o.OrderStatus == OrderStatus.Approved).ToList(),

			nameof(PaymentStatus.Pending) => orderHeaderResponseList
				.Where(o => o.PaymentStatus == PaymentStatus.Pending
				|| o.PaymentStatus == PaymentStatus.DelayedPayment).ToList(),

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
				OrderSummaryResponse = await ServiceUnitOfWork.OrderSummaryService
				.GetAsync(o => o.OrderHeaderId == orderHeaderResponse.Id, cancellationToken: cancellationToken)
			};

			orderVMs.Add(orderVM);
		}

		return Json(new { data = orderVMs });
	}
	#endregion
}
