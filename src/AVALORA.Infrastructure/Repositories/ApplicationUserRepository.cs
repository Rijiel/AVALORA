using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class ApplicationUserRepository : GenericRepository<ApplicationUser>, IApplicationUserRepository
{
    public ApplicationUserRepository(ApplicationDbContext dbContext) : base(dbContext)
    {
    }
}

