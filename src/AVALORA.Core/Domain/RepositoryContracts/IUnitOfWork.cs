namespace AVALORA.Core.Domain.RepositoryContracts;

/// <summary>
/// Defines the unit of work for the repository layer.
/// </summary>
public interface IUnitOfWork
{
    public ICategoryRepository Categories { get; }
    public IProductRepository Products { get; }
    public IProductImageRepository ProductImages { get; }
    public IApplicationUserRepository ApplicationUsers { get; }
    public ICartItemRepository CartItems { get; }

    /// <summary>
    /// Saves changes made in the unit of work asynchronously.
    /// </summary>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task SaveAsync();
}

