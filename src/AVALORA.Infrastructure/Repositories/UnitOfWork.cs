using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;

namespace AVALORA.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _dbContext;

    public ICategoryRepository Categories { get; private set; }

    public UnitOfWork(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;

        Categories = new CategoryRepository(_dbContext);
    }

    public async Task SaveAsync()
    {
        await _dbContext.SaveChangesAsync();
    }
}

