using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.Dto.ProductReviewDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace AVALORA.Core.Services.FacadeServices;

public class ProductFacade : BaseFacade<ProductFacade>, IProductFacade
{
	private readonly IPagerFacade _pagerFacade;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly UserManager<IdentityUser> _userManager;

	public ProductFacade(IServiceProvider serviceProvider, IPagerFacade pagerFacade, 
		IHttpContextAccessor contextAccessor, UserManager<IdentityUser> userManager) : base(serviceProvider)
	{
		_pagerFacade = pagerFacade;
		_contextAccessor = contextAccessor;
		_userManager = userManager;
	}

	public async Task CreateProductAsync(ProductAddRequest? productAddRequest)
	{
		ProductResponse productResponse = await ServiceUnitOfWork.ProductService.AddAsync(productAddRequest);
		ProductUpdateRequest productUpdateRequest = Mapper.Map<ProductUpdateRequest>(productResponse);

		// Check if there are any image files in the request
		if (productAddRequest!.ImageFiles?.Count() > 0)
		{
			List<ProductImageResponse> productImageResponses = await ServiceUnitOfWork.ProductImageService
                .CreateImagesAsync(productResponse.Id, productAddRequest.ImageFiles);

			productUpdateRequest.ProductImages = Mapper.Map<List<ProductImage>>(productImageResponses);

			Logger.LogInformation("Created images for product {productId}", productResponse.Id);
		}

		// Update the product to contain the images after creating them
		await ServiceUnitOfWork.ProductService.UpdateAsync(productUpdateRequest);

		Logger.LogInformation("Created product {productId}", productResponse.Id);
	}

	public async Task UpdateProductAsync(ProductUpdateRequest? productUpdateRequest)
	{
		if (productUpdateRequest != null)
		{
			// Create product images if image files exist
			if (productUpdateRequest.ImageFiles?.Count() > 0)
			{
				List<ProductImageResponse> productImageResponses = await ServiceUnitOfWork.ProductImageService
                    .CreateImagesAsync(productUpdateRequest.Id, productUpdateRequest.ImageFiles);

				productUpdateRequest.ProductImages = Mapper.Map<List<ProductImage>>(productImageResponses);

				Logger.LogInformation("Created {productImagesCount} images for product: {productId}",
                    productImageResponses.Count, productUpdateRequest.Id);
			}

			await ServiceUnitOfWork.ProductService.UpdateAsync(productUpdateRequest);

			Logger.LogInformation("Updated product {productId}", productUpdateRequest.Id);
		}
	}

	public async Task DeleteProductAsync(int? id)
	{
		await ServiceUnitOfWork.ProductService.RemoveAsync(id);

		// Delete product images
		await ServiceUnitOfWork.ProductImageService.DeleteAllImagesAsync(id);

		Logger.LogInformation("Deleted product {productId} and its product images", id);
	}

	public async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync(CancellationToken cancellationToken = default)
	{
		List<CategoryResponse> categoryResponses = await ServiceUnitOfWork.CategoryService
            .GetAllAsync(cancellationToken: cancellationToken);

		return categoryResponses.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
	}

	public async Task<int> GetProductImageCount(int? productId, CancellationToken cancellationToken = default)
	{
		int count = 0;

		if (productId != null)
		{
			IEnumerable<ProductImageResponse> productImageResponses = await ServiceUnitOfWork.ProductImageService
                .GetAllAsync(p => p.ProductId == productId, cancellationToken: cancellationToken);

			count = productImageResponses.Count();
		}

		// If we got this far, something failed, return 0
		return count;
	}

    public List<ProductResponse> GetFilteredProducts(List<ProductResponse> productResponses, 
        string? category = null, string? color = null, string? search = null)
    {
        List<ProductResponse> filteredProducts = productResponses;

        var filters = new Func<ProductResponse, bool>[]
        {
            !String.IsNullOrEmpty(category) ? p => p.Category?.Name == category : p => true,
            !String.IsNullOrEmpty(color) ? p => p.Colors.Contains(Enum.Parse<Color>(color)) : p => true,
            !String.IsNullOrEmpty(search) ? p => p.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase) : p => true
        };

        filteredProducts = filteredProducts
            .Where(p => filters.All(f => f(p))).ToList();

        return filteredProducts;
    }

	public async Task<ProductDetailsVM> GetProductDetailsVMAsync(Controller controller, int? id, int page,
		CancellationToken cancellationToken = default)
	{
		// Get product to show details
		ProductResponse? productResponse = await ServiceUnitOfWork.ProductService
			.GetByIdAsync(id, cancellationToken: cancellationToken,
			includes: [nameof(ProductResponse.Category),nameof(ProductResponse.ProductImages),
				nameof(ProductResponse.ProductReviews)]);

		if (productResponse == null)
		{
			Logger.LogWarning("Product {productId} not found", id);
			throw new KeyNotFoundException("Product not found.");
		}

		productResponse.TotalRating = await ServiceUnitOfWork.ProductService
			.GetTotalRatingAsync(productResponse.Id, cancellationToken);

		var pagedProductReviewResponses = await _pagerFacade
			.GetPagedProductReviews(page, 3, productResponse.Id, cancellationToken);

		controller.ViewBag.Pager = ServiceUnitOfWork.PagerService;

		// Use CartItemAddRequestVM to combine CartItemAddRequest and product responses
		var cartItemAddRequestVM = new CartItemAddRequestVM()
		{
			ProductResponse = productResponse,

			// Provide a valid cart item for model validation with product id
			CartItemAddRequest = new CartItemAddRequest()
			{
				ApplicationUserId = UserHelper.GetCurrentUserId(_contextAccessor)!,
				ProductId = productResponse.Id
			}
		};

		// Use ProductReviewVM to combine ProductReviewAddRequest and product review responses
		var productReviewVM = new ProductReviewVM()
		{
			ProductReviewAddRequest = new ProductReviewAddRequest()
			{
				ProductId = productResponse.Id,
				DatePosted = DateTime.Now,
				UserName = _userManager.GetUserName(controller.User)!
			},
			ProductReviewResponses = pagedProductReviewResponses
		};

		// Use ProductDetailsVM to combine CartItemAddRequestVM and ProductReviewVM
		var productDetailsVM = new ProductDetailsVM()
		{
			CartItemAddRequestVM = cartItemAddRequestVM,
			ProductReviewVM = productReviewVM
		};

		return productDetailsVM;
	}
}

