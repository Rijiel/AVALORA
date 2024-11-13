using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class OrderHeaderService : GenericService<OrderHeader, OrderHeaderAddRequest, OrderHeaderUpdateRequest, 
    OrderHeaderResponse>, IOrderHeaderService
{
	public OrderHeaderService(IOrderHeaderRepository repository, IMapper mapper, IUnitOfWork unitOfWork) 
        : base(repository, mapper, unitOfWork)
    {
	}

	public OrderHeaderAddRequest SetOrderHeaderDefaults(OrderHeaderAddRequest addRequest)
	{
		addRequest.OrderStatus = OrderStatus.Pending;
		addRequest.OrderDate = DateTime.Now;
		addRequest.PaymentStatus = PaymentStatus.Pending;
		addRequest.PaymentDueDate = DateTime.Now.AddDays(1);

		return addRequest;
	}

	public async Task<OrderHeaderResponse> UpdateOrderStatusAsync(int? id, OrderStatus status, 
        PaymentStatus? paymentStatus = null)
	{
		OrderHeaderResponse? orderHeaderResponse = await GetByIdAsync(id)
			?? throw new KeyNotFoundException("Order header not found.");

		// Set order status and payment status of order from db
		orderHeaderResponse.OrderStatus = status;

		if (status == OrderStatus.Shipped)
			orderHeaderResponse.ShippingDate = DateTime.Now;

		if (paymentStatus != null)
			orderHeaderResponse.PaymentStatus = paymentStatus.Value;
		
		var orderHeaderUpdateRequest = Mapper.Map<OrderHeaderUpdateRequest>(orderHeaderResponse);
		await UpdateAsync(orderHeaderUpdateRequest);

		return orderHeaderResponse;
	}

	public async Task<OrderHeaderResponse> UpdatePaymentIdAsync(int? id, string? paymentId)
	{
		if (String.IsNullOrEmpty(paymentId))
			throw new ArgumentNullException();

		OrderHeaderResponse? orderHeaderResponse = await GetByIdAsync(id)
			?? throw new KeyNotFoundException("Order not found.");

		orderHeaderResponse.PaymentID = paymentId;
		orderHeaderResponse.PaymentDate = DateTime.Now;
		orderHeaderResponse.PaymentDueDate = null;

		var orderHeaderUpdateRequest = Mapper.Map<OrderHeaderUpdateRequest>(orderHeaderResponse);
		await UpdateAsync(orderHeaderUpdateRequest);

		return orderHeaderResponse;
	}
}

