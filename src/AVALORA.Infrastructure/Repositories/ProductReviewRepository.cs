using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class ProductReviewRepository : GenericRepository<ProductReview>, IProductReviewRepository
{
    public ProductReviewRepository(ApplicationDbContext dbContext) : base(dbContext)
    {        
    }
}

