using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class OrderHeaderRepository : GenericRepository<OrderHeader>, IOrderHeaderRepository
{
    public OrderHeaderRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

