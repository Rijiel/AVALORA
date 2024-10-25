using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class OrderSummaryItemRepository : GenericRepository<OrderSummaryItem>, IOrderSummaryItemRepository
{
    public OrderSummaryItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {        
    }
}

