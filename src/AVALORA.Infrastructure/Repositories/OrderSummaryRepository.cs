using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class OrderSummaryRepository : GenericRepository<OrderSummary>, IOrderSummaryRepository
{
    public OrderSummaryRepository(ApplicationDbContext dbContext) : base(dbContext)
    {        
    }
}

