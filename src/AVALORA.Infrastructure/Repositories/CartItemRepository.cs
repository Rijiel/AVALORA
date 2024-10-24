using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class CartItemRepository : GenericRepository<CartItem>, ICartItemRepository
{
    public CartItemRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

