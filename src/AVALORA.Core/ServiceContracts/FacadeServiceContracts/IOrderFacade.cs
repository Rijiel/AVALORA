using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Core.ServiceContracts.FacadeServiceContracts;

public interface IOrderFacade
{
	/// <summary>
	/// Retrieves the current user's order header add request asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token that can be used by 
	/// other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation,
	/// containing the current user's order header add request.</returns>
	Task<OrderHeaderAddRequest> CreateOrderHeaderAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Persist the current user's order header add request asynchronously.
	/// </summary>
	/// <param name="orderHeaderAddRequest">The order header add request.</param>
	/// <param name="orderSummaryAddRequest">The order summary add request.</param>
	/// <param name="controller">The controller instance.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by 
	/// other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation,
	/// containing the order header response and TempData message.</returns>
	Task<OrderHeaderResponse> PlaceOrderAsync(OrderHeaderAddRequest orderHeaderAddRequest, OrderSummaryAddRequest orderSummaryAddRequest,
		Controller controller, CancellationToken cancellationToken = default);

	/// <summary>
	/// Asynchronously creates an order summary.
	/// </summary>
	/// <param name="addRequest">The order summary add request.</param>
	/// <param name="orderHeaderId">The ID of the order header. Optional.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads 
	/// to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation, containing the order summary response.</returns>
	Task<OrderSummaryResponse> CreateOrderSummaryAsync(OrderSummaryAddRequest addRequest, int? orderHeaderId = null, 
		CancellationToken cancellationToken = default);
}

