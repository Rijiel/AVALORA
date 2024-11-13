using System.Linq.Expressions;

namespace AVALORA.Core.Domain.RepositoryContracts;

public interface IGenericRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// This method retrieves all entities from the repository asynchronously.
    /// </summary>
    /// <param name="filter">An optional filter expression to filter the entities.</param>
    /// <param name="includes">An optional list of navigation properties to include in the result.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entities.</returns>
    Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>>? filter = null, params string[] includes);

    /// <summary>
    /// Retrieves a single entity from the repository asynchronously based on the specified filter.
    /// </summary>
    /// <param name="filter">The filter expression used to find the entity.</param>
    /// <param name="includes">An optional list of navigation properties to include in the result.</param>
    /// <param name="tracked">Specifies whether the entity should be tracked by the context. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entity, or null 
    /// if no entity is found.</returns>
    Task<TEntity?> GetAsync(Expression<Func<TEntity, bool>> filter, bool tracked = false, params string[] includes);

    /// <summary>
    /// Retrieves a single entity from the repository asynchronously based on the specified ID.
    /// </summary>
    /// <param name="id">The ID of the entity to retrieve.</param>
    /// <param name="includes">An optional list of navigation properties to include in the result.</param>
    /// <param name="tracked">Specifies whether the entity should be tracked by the context. Default is false.</param>
    /// <returns>A task that represents the asynchronous operation. The task result contains the retrieved entity, or null 
    /// if no entity is found.</returns>
    Task<TEntity?> GetByIdAsync(object id, bool tracked = false, params string[] includes);

    /// <summary>
    /// Adds a new entity to the repository.
    /// </summary>
    /// <param name="entity">The entity to be added.</param>
    /// <remarks>
    /// This method adds a new entity to the repository. It does not persist the changes to the database.
    /// </remarks>
    void Add(TEntity entity);

    /// <summary>
    /// Adds a range of entities to the repository.
    /// </summary>
    /// <param name="entities">The entities to be added.</param>
    /// <remarks>
    /// This method adds a range of entities to the repository. It does not persist the changes to the database.
    /// </remarks>
    void AddRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    /// <remarks>
    /// This method removes an entity from the repository. It does not persist the changes to the database.
    /// </remarks>
    void Remove(TEntity entity);

    /// <summary>
    /// Removes a range of entities from the repository.
    /// </summary>
    /// <param name="entities">The entities to be removed.</param>
    /// <remarks>
    /// This method removes a range of entities from the repository. It does not persist the changes to the database.
    /// </remarks>
    void RemoveRange(IEnumerable<TEntity> entities);
}

