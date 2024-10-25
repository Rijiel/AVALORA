using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.OrderHeaderDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IOrderHeaderSevice : IGenericService<OrderHeader, OrderHeaderAddRequest, OrderHeaderUpdateRequest, OrderHeaderResponse>
{
}

