using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Enums;
using Microsoft.AspNetCore.Http;

namespace AVALORA.Core.ServiceContracts;

public interface IOrderHeaderService : IGenericService<OrderHeader, OrderHeaderAddRequest, OrderHeaderUpdateRequest, OrderHeaderResponse>
{
	/// <summary>
	/// Sets the default values for the order header.
	/// </summary>
	/// <param name="addRequest">The order header add request.</param>
	/// <param name="contextAccessor">The HTTP context accessor.</param>
	/// <returns>The order header add request with default values set.</returns>
	OrderHeaderAddRequest SetOrderHeaderDefaults(OrderHeaderAddRequest addRequest);

	/// <summary>
	/// Updates the status of an order.
	/// </summary>
	/// <param name="id">The ID of the order to update.</param>
	/// <param name="status">The new status of the order.</param>
	/// <param name="paymentStatus">The payment status of the order (optional).</param>
	/// or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation, containing the updated order header response.</returns>
	Task<OrderHeaderResponse> UpdateOrderStatusAsync(int? id, OrderStatus status, PaymentStatus? paymentStatus = null);

	/// <summary>
	/// Updates the payment ID of an order.
	/// </summary>
	/// <param name="id">The ID of the order to update.</param>
	/// <param name="paymentId">The new payment ID of the order.</param>
	/// or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation, containing the updated order header response.</returns>
	Task<OrderHeaderResponse> UpdatePaymentIdAsync(int? id, string? paymentId);
}

