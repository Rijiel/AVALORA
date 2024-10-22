namespace AVALORA.Core.Domain.RepositoryContracts;

public interface IUnitOfWork
{
    public ICategoryRepository Categories { get; }

    /// <summary>
    /// Saves changes made in the unit of work asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync();
}

