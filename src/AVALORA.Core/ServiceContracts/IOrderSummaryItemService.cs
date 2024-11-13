using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.OrderSummaryItemDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IOrderSummaryItemService : IGenericService<OrderSummaryItem, OrderSummaryItemAddRequest, 
    OrderSummaryItemUpdateRequest, OrderSummaryItemResponse>
{
}

