using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ApplicationUserDtos;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Dto.OrderSummaryItemDtos;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AVALORA.Core.Services.FacadeServices;

public class OrderFacade : BaseFacade<OrderFacade>, IOrderFacade
{
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly ICartFacade _cartFacade;

	public OrderFacade(IHttpContextAccessor contextAccessor, IServiceProvider serviceProvider, ICartFacade cartFacade) : base(serviceProvider)
    {
		_contextAccessor = contextAccessor;
		_cartFacade = cartFacade;
	}

    public async Task<OrderHeaderAddRequest> CreateOrderHeaderAsync(CancellationToken cancellationToken = default)
	{
		string userId = UserHelper.GetCurrentUserId(_contextAccessor)!;
		ApplicationUserResponse? applicationUser = await ServiceUnitOfWork.ApplicationUserService
			.GetByIdAsync(userId, cancellationToken: cancellationToken)
			?? throw new KeyNotFoundException($"Application user not found with id {userId}");

		// Match user information to order header request
		var orderHeaderAddRequest = Mapper.Map<OrderHeaderAddRequest>(applicationUser);

		// Users have a default 7-day estimated delivery date
		orderHeaderAddRequest.EstimatedFromDate = DateTime.Now.AddDays(7);
		orderHeaderAddRequest.EstimatedToDate = DateTime.Now.AddDays(14);

		Logger.LogInformation($"Created order header");
		return orderHeaderAddRequest;
	}

	public async Task<OrderHeaderResponse> PlaceOrderAsync(OrderHeaderAddRequest orderHeaderAddRequest, 
		OrderSummaryAddRequest orderSummaryAddRequest, Controller controller, CancellationToken cancellationToken = default)
	{
		// Populate order header properties
		orderHeaderAddRequest = ServiceUnitOfWork.OrderHeaderSevice.SetOrderHeaderDefaults(orderHeaderAddRequest, _contextAccessor);

		OrderHeaderResponse orderHeaderResponse = await ServiceUnitOfWork.OrderHeaderSevice.AddAsync(orderHeaderAddRequest);

		await CreateOrderSummaryAsync(orderSummaryAddRequest, orderHeaderResponse.Id, cancellationToken);
		controller.TempData[SD.TEMPDATA_SUCCESS] = "Order has been placed!";

		Logger.LogInformation($"Placed order for user: {UserHelper.GetCurrentUserId(_contextAccessor)}");
		return orderHeaderResponse;
	}

	public async Task<OrderSummaryResponse> CreateOrderSummaryAsync(OrderSummaryAddRequest addRequest, 
		int? orderHeaderId = null, CancellationToken cancellationToken = default)
	{
		// Ensure order header ID and add order summary
		addRequest.OrderHeaderId = orderHeaderId ?? throw new ArgumentNullException(nameof(orderHeaderId));
		OrderSummaryResponse orderSummaryResponse = await ServiceUnitOfWork.OrderSummaryService.AddAsync(addRequest);

		// Map order summary response to update request and associate with cart items
		var orderSummaryUpdateRequest = Mapper.Map<OrderSummaryUpdateRequest>(orderSummaryResponse);
		List<CartItemResponse> cartItemResponseList = await _cartFacade
			.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);

		// Add order summary items and update order summary
		var orderSummaryItemAddList = Mapper.Map<List<OrderSummaryItemAddRequest>>(cartItemResponseList);
		foreach (var orderSummaryItemAdd in orderSummaryItemAddList)
			orderSummaryItemAdd.OrderSummaryId = orderSummaryResponse.Id;

		List<OrderSummaryItemResponse> orderSummaryItemResponseList = await ServiceUnitOfWork.OrderSummaryItemService
			.AddRangeAsync(orderSummaryItemAddList);

		var orderSummaryItems = Mapper.Map<List<OrderSummaryItem>>(orderSummaryItemResponseList);
		orderSummaryUpdateRequest.OrderSummaryItems = orderSummaryItems;

		await ServiceUnitOfWork.OrderSummaryService.UpdateAsync(orderSummaryUpdateRequest);

		Logger.LogInformation($"Created order summary for order: {orderHeaderId}");
		return orderSummaryResponse;
	}
}

