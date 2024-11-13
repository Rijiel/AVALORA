using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class OrderSummaryService : GenericService<OrderSummary, OrderSummaryAddRequest, OrderSummaryUpdateRequest, 
    OrderSummaryResponse>, IOrderSummaryService
{
    public OrderSummaryService(IOrderSummaryRepository repository, IMapper mapper, IUnitOfWork unitOfWork) 
        : base(repository, mapper, unitOfWork)
    {        
    }
}

