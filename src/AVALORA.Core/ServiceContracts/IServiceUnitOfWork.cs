namespace AVALORA.Core.ServiceContracts;

/// <summary>
/// Defines the unit of work interface for the service layer.
/// </summary>
public interface IServiceUnitOfWork
{
    public ICategoryService CategoryService { get; }
    public IProductService ProductService { get; }
    public IProductImageService ProductImageService { get; }
    public IApplicationUserService ApplicationUserService { get; }
    public ICartItemService CartItemService { get; }
    public IOrderHeaderService OrderHeaderService { get; }
    public IOrderSummaryService OrderSummaryService { get; }
    public IOrderSummaryItemService OrderSummaryItemService { get; }
    public IProductReviewService ProductReviewService { get; }
    public IPagerService PagerService { get; }
    public IPaymentService PaymentService { get; }
    public IBreadcrumbService BreadcrumbService { get; }
}

