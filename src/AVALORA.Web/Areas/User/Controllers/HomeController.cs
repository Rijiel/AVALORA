using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductReviewDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using AVALORA.Web.Filters.ActionFilters;
using AVALORA.Web.Filters.ResultFIlters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;
using System.Diagnostics;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Route("[controller]/[action]")]
[DefaultBreadcrumb("AVALORA", AreaName = nameof(Role.User))]
public class HomeController : BaseController<HomeController>
{
	private readonly ICartFacade _cartFacade;
	private readonly IProductFacade _productFacade;
	private readonly IPagerFacade _pagerFacade;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly UserManager<IdentityUser> _userManager;

	public HomeController(ICartFacade cartFacade, IProductFacade productFacade, IPagerFacade pagerFacade, 
		IHttpContextAccessor contextAccessor, UserManager<IdentityUser> userManager)
	{
		_cartFacade = cartFacade;
		_productFacade = productFacade;
		_pagerFacade = pagerFacade;
		_contextAccessor = contextAccessor;
		_userManager = userManager;
	}

	// GET: /?category={category}&color={color}&page={page}&search={search}
	// GET: /Home?category={category}&color={color}&page={page}&search={search}
	[Route("/")]
	[Route("/Home")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken, [FromQuery] string? category = null,
		[FromQuery] string? color = null, [FromQuery] string? search = null, [FromQuery] int page = 1)
	{
		// Use unflitered product responses for sidebar
		var productResponses = await ServiceUnitOfWork.ProductService.GetAllAsync(cancellationToken: cancellationToken,
			includes: [nameof(ProductResponse.ProductImages), nameof(ProductResponse.Category)]);

		// Populate total rating
		foreach (var product in productResponses)
			product.TotalRating = await ServiceUnitOfWork.ProductService.GetTotalRatingAsync(product.Id, cancellationToken);

		// Separate filtered products for searching, filtering and paging
		var filteredProducts = _productFacade.GetFilteredProducts(productResponses, category, color, search);

		// Paging implementation
		List<ProductResponse> pagedProductResponses = ServiceUnitOfWork.PagerService
			.GetPagedItems(filteredProducts, page, pageSize: 12);

		ViewBag.Pager = ServiceUnitOfWork.PagerService;
		ViewBag.Category = category;
		ViewBag.Color = color;
		ViewBag.Search = search;

		Logger.LogInformation("Loaded {productsCount} products", pagedProductResponses.Count);

		// Get category counts ordered by number of products having the category name
		var categoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync(cancellationToken: cancellationToken);
		var categoryCounts = categoryResponses
			.OrderByDescending(c => productResponses.Count(p => p.CategoryId == c.Id))
			.ToDictionary(c => c.Name, c => productResponses.Count(p => p.CategoryId == c.Id));

		// Use ProductsCategoriesVM to combine ProductResponses and CategoryResponses with total products per category
		var productsCategoriesVM = new ProductsCategoriesVM
		{
			ProductResponses = productResponses,
			FilteredProducts = pagedProductResponses,
			CategoryCounts = categoryCounts
		};

		return View(productsCategoriesVM);
	}

	// GET: /Home/Details/{id}?page={page}
	[HttpGet("{id?}")]
	public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken, [FromQuery] int page = 1)
	{
		ProductDetailsVM productDetailsVM = await _productFacade.GetProductDetailsVMAsync(this, id, page, cancellationToken);

		var productResponse = await ServiceUnitOfWork.ProductService
			.GetByIdAsync(id, includes: [nameof(ProductResponse.Category)], cancellationToken: cancellationToken);

		if (productResponse != null)
		{
			ServiceUnitOfWork.BreadcrumbService.SetCustomNodes(this, "Home",
			controllerActions: [nameof(Index), nameof(Details)],
			titles: [productResponse.Category.Name, productResponse.Name],
			routeValues: [
				new() { { "category", productResponse.Category.Name } },
				]);
		}

		return View(productDetailsVM);
	}

	// POST: /Home/Details/{id}
	[Authorize]
	[HttpPost("{id?}")]
	[ActionName(nameof(Details))]
	[TypeFilter(typeof(ValidationActionFilter), Arguments = [true])]
	[TypeFilter(typeof(HomeControllerResultFilter))]
	public async Task<IActionResult> AddToCart(CartItemAddRequestVM cartItemAddRequestVM, CancellationToken cancellationToken)
	{
		await _cartFacade.AddToCartAsync(cartItemAddRequestVM.CartItemAddRequest, this);
		return RedirectToAction(nameof(Index));
	}

	// POST: /Home/AddReview
	[HttpPost]
	public async Task<IActionResult> AddReview(ProductReviewVM productReviewVM, CancellationToken cancellationToken)
	{
		ProductReviewAddRequest productReviewAddRequest = productReviewVM.ProductReviewAddRequest;

		if (ModelState.IsValid)
		{
			await ServiceUnitOfWork.ProductReviewService.AddAsync(productReviewAddRequest);

			// Update Product total rating and ratings count
			var productUpdateRequest = new ProductUpdateRequest()
			{
				Id = productReviewAddRequest.ProductId,
				TotalRating = await ServiceUnitOfWork.ProductService
				.GetTotalRatingAsync(productReviewAddRequest.ProductId, cancellationToken)
			};

			await ServiceUnitOfWork.ProductService.UpdatePartialAsync(productUpdateRequest, nameof(Product.TotalRating));

			SuccessMessage = "Review added, thanks!";
			Logger.LogInformation("Added review for product {productId}", productReviewAddRequest.ProductId);
		}
		else
			ErrorMessage = "Please select your rating";

		// If we got this far, something failed, redisplay form
		return RedirectToAction(nameof(Details), new { id = productReviewAddRequest.ProductId });
	}

	// GET: /Home/Privacy
	public IActionResult Privacy()
	{
		return View();
	}

	// GET: /Home/Error
	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
