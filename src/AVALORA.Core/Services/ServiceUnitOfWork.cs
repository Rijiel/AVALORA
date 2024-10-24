using AutoMapper;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Hosting;

namespace AVALORA.Core.Services;

/// <summary>
/// Represents a unit of work for the service layer, encapsulating access to various services.
/// </summary>
public class ServiceUnitOfWork : IServiceUnitOfWork
{
    public ICategoryService CategoryService { get; private set; }
    public IProductService ProductService{ get; private set; }
    public IProductImageService ProductImageService { get; private set; }
    public IApplicationUserService ApplicationUserService { get; private set; }
    public ICartItemService CartItemService { get; private set; }

    public ServiceUnitOfWork(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        CategoryService = new CategoryService(unitOfWork.Categories, mapper, unitOfWork);
        ProductService = new ProductService(unitOfWork.Products, mapper, unitOfWork);
		ProductImageService = new ProductImageService(unitOfWork.ProductImages, mapper, unitOfWork, webHostEnvironment);
        ApplicationUserService = new ApplicationUserService(unitOfWork.ApplicationUsers, mapper, unitOfWork);
        CartItemService = new CartItemService(unitOfWork.CartItems, mapper, unitOfWork);
    }
}

