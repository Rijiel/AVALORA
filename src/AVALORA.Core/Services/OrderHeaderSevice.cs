using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class OrderHeaderSevice : GenericService<OrderHeader, OrderHeaderAddRequest, OrderHeaderUpdateRequest, OrderHeaderResponse>, IOrderHeaderSevice
{
    public OrderHeaderSevice(IOrderHeaderRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : base(repository, mapper, unitOfWork)
    {        
    }
}

