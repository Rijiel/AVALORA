using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace AVALORA.Core.Services;

public class OrderHeaderSevice : GenericService<OrderHeader, OrderHeaderAddRequest, OrderHeaderUpdateRequest, OrderHeaderResponse>, IOrderHeaderSevice
{
	private readonly UserManager<IdentityUser> _userManager;

	public OrderHeaderSevice(IOrderHeaderRepository repository, IMapper mapper, IUnitOfWork unitOfWork, UserManager<IdentityUser> userManager) : base(repository, mapper, unitOfWork)
    {
		_userManager = userManager;
	}

	public OrderHeaderAddRequest SetOrderHeaderDefaults(OrderHeaderAddRequest addRequest, IHttpContextAccessor contextAccessor)
	{
		addRequest.OrderStatus = OrderStatus.Pending;
		addRequest.OrderDate = DateTime.Now;
		addRequest.PaymentStatus = PaymentStatus.Pending;
		addRequest.PaymentDueDate = DateTime.Now.AddDays(1);

		return addRequest;
	}

	public async Task<OrderHeaderResponse> UpdateOrderStatusAsync(int? id, OrderStatus status, PaymentStatus? paymentStatus = null, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(id);

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
}

