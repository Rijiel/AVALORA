using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Text.Json.Nodes;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Authorize]
[Route("[controller]/[action]")]
public class PaymentController : BaseController<PaymentController>
{
	private readonly ICartFacade _cartFacade;
	private readonly IOptions<PaypalSettings> _paypal;
	private readonly IHttpContextAccessor _contextAccessor;

	public PaymentController(ICartFacade cartFacade, IOptions<PaypalSettings> paypal, IHttpContextAccessor contextAccessor)
	{
		_cartFacade = cartFacade;
		_paypal = paypal;
		_contextAccessor = contextAccessor;
	}

	public async Task<IActionResult> Index(int? id, CancellationToken cancellationToken)
	{
		OrderHeaderResponse? orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderService.GetByIdAsync(id ?? 0);

		// Only allow payment for current user's orders
		if (orderHeaderResponse != null && orderHeaderResponse.ApplicationUserId == UserHelper.GetCurrentUserId(_contextAccessor))
		{
			// Only allow payment for pending, cancelled and delayed payment orders
			if (orderHeaderResponse.OrderStatus == OrderStatus.Pending
			|| (orderHeaderResponse.PaymentStatus == PaymentStatus.Cancelled && orderHeaderResponse.OrderStatus != OrderStatus.Cancelled)
			|| (orderHeaderResponse.OrderStatus == OrderStatus.Shipped && orderHeaderResponse.PaymentStatus == PaymentStatus.DelayedPayment))
			{
				HttpContext.Session.SetInt32(SD.TEMPDATA_ORDERID, id ?? 0);

				ViewBag.ClientID = _paypal.Value.ClientID;

				OrderSummaryResponse? orderSummaryResponse = await ServiceUnitOfWork.OrderSummaryService
					.GetAsync(o => o.OrderHeaderId == id);

				if (orderSummaryResponse == null)
				{
					Logger.LogWarning("Order not found");
					return NotFound("Order not found");
				}

				// Ensure total price has 2 decimal places
				orderSummaryResponse.TotalPrice = Math.Round(orderSummaryResponse.TotalPrice, 2);

				// Only empty cart when checking out
				if (TempData[SD.TEMPDATA_CLEARCART] is bool == true)
					await _cartFacade.ClearCartItemsAsync(this, cancellationToken);

				return View(orderSummaryResponse);
			}
		}

		Logger.LogError($"Invalid payment attempt for order id: {id}");
		throw new InvalidOperationException("Order cannot be paid at this moment.");
	}

	[HttpPost]
	public async Task<IActionResult> Create([FromBody] JsonObject data)
	{
		if (!data.ContainsKey("amount") || !decimal.TryParse(data["amount"]?.ToString(), out decimal totalAmount))
			return new JsonResult(new { Id = "", message = "Invalid amount or missing amount." });

		// create request body
		var createOrderRequest = new JsonObject
		{
			{ "intent", "CAPTURE" },
			{ "purchase_units", new JsonArray
				{
					new JsonObject
					{
						{ "amount", new JsonObject
							{
								{ "currency_code", "USD" },
								{ "value", totalAmount.ToString("F2") }
							}
						}
					}
				}
			}
		};

		string url = _paypal.Value.SandboxURL + "/v2/checkout/orders";
		string authHeaderValue = "Bearer " + await ServiceUnitOfWork.PaymentService.GetPaypalAccessTokenAsync(_paypal.Value);
		var httpContent = new StringContent(createOrderRequest.ToString(), null, "application/json");

		var jsonResponse = await ServiceUnitOfWork.PaymentService.SendRequestAsync(url, authHeaderValue, httpContent);

		if (jsonResponse != null)
		{
			string paypalOrderId = jsonResponse["id"]?.ToString() ?? "";

			return new JsonResult(new { Id = paypalOrderId });
		}

		return new JsonResult(new { Id = "" });
	}

	public async Task<JsonResult> Complete([FromBody] JsonObject data)
	{
		var orderId = data?["orderID"]?.ToString();
		if (String.IsNullOrEmpty(orderId))
			return new JsonResult("error");

		string url = _paypal.Value.SandboxURL + $"/v2/checkout/orders/{orderId}/capture";
		string authHeaderValue = "Bearer " + await ServiceUnitOfWork.PaymentService.GetPaypalAccessTokenAsync(_paypal.Value);
		var httpContent = new StringContent("", null, "application/json");

		var jsonResponse = await ServiceUnitOfWork.PaymentService.SendRequestAsync(url, authHeaderValue, httpContent);

		if (jsonResponse != null)
		{
			string paypalOrderStatus = jsonResponse["status"]?.ToString() ?? "";
			if (paypalOrderStatus == "COMPLETED")
			{
				string? paymentId = jsonResponse["purchase_units"]?[0]?["payments"]?["captures"]?[0]?["id"]?.ToString();
				TempData["PaymentId"] = paymentId;

				var redirectUrl = Url.Action(nameof(OrderConfirmation), "Payment");
				return new JsonResult(new { success = true, url = redirectUrl });
			}
		}

		return new JsonResult(new { success = false, url = "" });
	}

	public async Task<IActionResult> OrderConfirmation(int? id)
	{
		var orderHeaderId = id ?? HttpContext.Session.GetInt32(SD.TEMPDATA_ORDERID);
		OrderHeaderResponse? orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderService.GetByIdAsync(orderHeaderId);
		if (orderHeaderResponse == null)
		{
			Logger.LogWarning("Order not found");
			return NotFound("Order not found");
		}

		var paymentId = TempData["PaymentId"] as string;
		await ServiceUnitOfWork.OrderHeaderService.UpdatePaymentIdAsync(orderHeaderId, paymentId);
		await ServiceUnitOfWork.OrderHeaderService.UpdateOrderStatusAsync(orderHeaderId, OrderStatus.Approved, PaymentStatus.Approved);

		return View(orderHeaderResponse);
	}
}
