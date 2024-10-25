using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.OrderSummaryItemDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class OrderSummaryItemService : GenericService<OrderSummaryItem, OrderSummaryItemAddRequest, OrderSummaryItemUpdateRequest, OrderSummaryItemResponse>, IOrderSummaryItemService
{
    public OrderSummaryItemService(IOrderSummaryItemRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : base(repository, mapper, unitOfWork)
    {        
    }
}

