using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Infrastructure.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace AVALORA.Infrastructure.Repositories;

public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
{
    private readonly ApplicationDbContext _dbContext;
    private readonly DbSet<TEntity> _dbSet;

    public GenericRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
        _dbSet = _dbContext.Set<TEntity>();
    }

    public async Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, params string[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        // Disable change tracking to improve performance and prevent errors
        query = query.AsNoTracking();

        // Filter results to match specific criteria if requested
        if (filter != null)
            query = query.Where(filter);

        // Eager-load related entities for efficient data retrieval
        if (includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.ToListAsync();
    }

    public async Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, bool tracked = false, params string[] includes)
    {
        IQueryable<TEntity> query = _dbSet;

        // Apply the filter to the query if one is provided
        query = query.Where(filter);

        // Disable change tracking for performance when not required
        query = tracked ? query : query.AsNoTracking();

        // Eager-load related entities for efficient data retrieval
        if (includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return await query.FirstOrDefaultAsync();
    }

    public async Task<TEntity?> GetByIdAsync(object id, bool tracked = false, params string[] includes)
    {
        IQueryable<TEntity> query = _dbSet;
        TEntity? entity = null;

        // Retrieve entity by Id, handling different Id types, add more types as needed
        if (id is int intId)        
            entity = await query.SingleOrDefaultAsync(x => EF.Property<int>(x, "Id") == intId);
        else if (id is string strId)
            entity = await query.SingleOrDefaultAsync(x => EF.Property<string>(x, "Id") == strId);
        else
            throw new ArgumentException("Invalid id type", nameof(id));

        // Disable change tracking for performance when not required
        query = tracked ? query : query.AsNoTracking();

        // Eager-load related entities for efficient data retrieval
        if (includes.Length > 0)
        {
            foreach (var include in includes)
                query = query.Include(include);
        }

        return entity;
    }

    public void Add(TEntity entity) => _dbSet.Add(entity);

    public void AddRange(IEnumerable<TEntity> entities) => _dbSet.AddRange(entities);

    public void Remove(TEntity entity) => _dbSet.Remove(entity);

    public void RemoveRange(IEnumerable<TEntity> entities) => _dbSet.RemoveRange(entities);
}

