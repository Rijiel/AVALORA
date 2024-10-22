namespace AVALORA.Core.ServiceContracts;

public interface IServiceUnitOfWork
{
    public ICategoryService CategoryService { get; }
}

