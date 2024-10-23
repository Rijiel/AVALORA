using AutoMapper;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Hosting;

namespace AVALORA.Core.Services;

public class ServiceUnitOfWork : IServiceUnitOfWork
{
    public ICategoryService CategoryService { get; private set; }
    public IProductService ProductService{ get; private set; }
    public IProductImageService ProductImageService { get; private set; }

    public ServiceUnitOfWork(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment)
    {
        CategoryService = new CategoryService(unitOfWork.Categories, mapper, unitOfWork);
        ProductService = new ProductService(unitOfWork.Products, mapper, unitOfWork);
		ProductImageService = new ProductImageService(unitOfWork.ProductImages, mapper, unitOfWork, webHostEnvironment);
    }
}

