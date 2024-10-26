using AutoMapper;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;

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
    public IOrderHeaderSevice OrderHeaderSevice { get; private set; }
    public IOrderSummaryService OrderSummaryService { get; private set; }
    public IOrderSummaryItemService OrderSummaryItemService { get; private set; }

    public ServiceUnitOfWork(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment, UserManager<IdentityUser> userManager)
    {
        CategoryService = new CategoryService(unitOfWork.Categories, mapper, unitOfWork);
        ProductService = new ProductService(unitOfWork.Products, mapper, unitOfWork);
		ProductImageService = new ProductImageService(unitOfWork.ProductImages, mapper, unitOfWork, webHostEnvironment);
        ApplicationUserService = new ApplicationUserService(unitOfWork.ApplicationUsers, mapper, unitOfWork);
        CartItemService = new CartItemService(unitOfWork.CartItems, mapper, unitOfWork);
        OrderHeaderSevice = new OrderHeaderSevice(unitOfWork.OrderHeaders, mapper, unitOfWork, userManager);
		OrderSummaryService = new OrderSummaryService(unitOfWork.OrderSummaries, mapper, unitOfWork);
		OrderSummaryItemService = new OrderSummaryItemService(unitOfWork.OrderSummaryItems, mapper, unitOfWork);
    }
}

