using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.OrderSummaryDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IOrderSummaryService : IGenericService<OrderSummary, OrderSummaryAddRequest, OrderSummaryUpdateRequest, 
    OrderSummaryResponse>
{
}

