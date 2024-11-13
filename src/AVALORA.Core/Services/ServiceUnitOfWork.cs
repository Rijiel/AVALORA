using AutoMapper;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Helpers;
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
    public IOrderHeaderService OrderHeaderService { get; private set; }
    public IOrderSummaryService OrderSummaryService { get; private set; }
    public IOrderSummaryItemService OrderSummaryItemService { get; private set; }
    public IProductReviewService ProductReviewService { get; private set; }
	public IPagerService PagerService { get; private set; }
    public IPaymentService PaymentService { get; private set; }
    public IBreadcrumbService BreadcrumbService { get; private set; }

	public ServiceUnitOfWork(IUnitOfWork unitOfWork, IMapper mapper, IWebHostEnvironment webHostEnvironment, 
        IHttpClientFactory httpClientFactory, RoleManager<IdentityRole> roleManager)
    {
        CategoryService = new CategoryService(unitOfWork.Categories, mapper, unitOfWork);
        ProductService = new ProductService(unitOfWork.Products, mapper, unitOfWork);
		ProductImageService = new ProductImageService(unitOfWork.ProductImages, mapper, unitOfWork, webHostEnvironment);
        ApplicationUserService = new ApplicationUserService(unitOfWork.ApplicationUsers, mapper, unitOfWork, roleManager);
        CartItemService = new CartItemService(unitOfWork.CartItems, mapper, unitOfWork);
        OrderHeaderService = new OrderHeaderService(unitOfWork.OrderHeaders, mapper, unitOfWork);
		OrderSummaryService = new OrderSummaryService(unitOfWork.OrderSummaries, mapper, unitOfWork);
		OrderSummaryItemService = new OrderSummaryItemService(unitOfWork.OrderSummaryItems, mapper, unitOfWork);
		ProductReviewService = new ProductReviewService(unitOfWork.ProductReviews, mapper, unitOfWork);
		PagerService = new PagerService();
		PaymentService = new PaymentService(httpClientFactory.CreateClient(SD.PAYMENT_CLIENT));
        BreadcrumbService = new BreadcrumbService();
    }
}

