namespace AVALORA.Core.ServiceContracts;

public interface IServiceUnitOfWork
{
    public ICategoryService CategoryService { get; }
    public IProductService ProductService { get; }
    public IProductImageService ProductImageService { get; }
}

