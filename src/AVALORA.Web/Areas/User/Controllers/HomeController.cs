using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductReviewDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Route("[controller]/[action]")]
public class HomeController : BaseController<HomeController>
{
	private readonly ICartFacade _cartFacade;
	private readonly IHttpContextAccessor _contextAccessor;
	private readonly UserManager<IdentityUser> _userManager;

	public HomeController(ICartFacade cartFacade, IHttpContextAccessor contextAccessor, UserManager<IdentityUser> userManager)
	{
		_cartFacade = cartFacade;
		_contextAccessor = contextAccessor;
		_userManager = userManager;
	}
	[Route("/")]
	[Route("/Home")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken, [FromQuery] string? category = null,
		[FromQuery] string? color = null, [FromQuery] int page = 1, [FromQuery] string? search = null)
	{
		// Use unflitered product responses for sidebar
		var productResponses = await ServiceUnitOfWork.ProductService
			.GetAllAsync(cancellationToken: cancellationToken,
			includes: [nameof(ProductResponse.ProductImages), nameof(ProductResponse.Category)]);

		// Populate total rating
		foreach (var product in productResponses)
			product.TotalRating = await ServiceUnitOfWork.ProductService
				.GetTotalRatingAsync(product.Id, cancellationToken);

		// Separate filtered products for searching, filtering and paging
		var filteredProducts = productResponses;

		// Filter by category
		if (!String.IsNullOrEmpty(category))
			filteredProducts = filteredProducts
				.Where(p => p.Category?.Name == category).ToList();

		// Filter by color
		if (!String.IsNullOrEmpty(color))
			filteredProducts = filteredProducts
				.Where(p => p.Colors.Contains(Enum.Parse<Color>(color))).ToList();

		// Search implementation
		if (!String.IsNullOrEmpty(search))
			filteredProducts = filteredProducts
				.Where(p => p.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase)).ToList();

		// Paging implementation
		List<ProductResponse> pagedProductResponses = ServiceUnitOfWork.PagerService
			.GetPagedItems(filteredProducts, page, pageSize: 12);
		ViewBag.Pager = ServiceUnitOfWork.PagerService;
		ViewBag.Category = category;
		ViewBag.Color = color;
		ViewBag.Search = search;

		Logger.LogInformation($"Loaded {pagedProductResponses.Count} products");

		// Get category counts ordered by number of products having the category name
		var categoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync(cancellationToken: cancellationToken);
		var categoryCounts = categoryResponses.OrderByDescending(c => productResponses.Count(p => p.CategoryId == c.Id))
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

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken, [FromQuery] int page = 1)
	{
		ProductResponse? productResponse = await ServiceUnitOfWork.ProductService
			.GetByIdAsync(id, cancellationToken: cancellationToken,
			includes: [nameof(ProductResponse.Category),
				nameof(ProductResponse.ProductImages),
				nameof(ProductResponse.ProductReviews)]);

		if (productResponse == null)
		{
			Logger.LogWarning("Product not found");
			return NotFound();
		}

		productResponse.TotalRating = await ServiceUnitOfWork.ProductService.GetTotalRatingAsync(productResponse.Id, cancellationToken);

		List<ProductReviewResponse> productReviewResponses = await ServiceUnitOfWork.ProductReviewService
			.GetAllAsync(p => p.ProductId == productResponse.Id, cancellationToken: cancellationToken);

		// Sort by comment length -> rating -> date posted
		productReviewResponses = productReviewResponses
			.OrderByDescending(p => p.Comment?.Length)
			.ThenByDescending(p => p.Rating)
			.ThenByDescending(p => p.DatePosted.Date).ToList();

		List<ProductReviewResponse> pagedProductReviewResponses = ServiceUnitOfWork.PagerService
			.GetPagedItems(productReviewResponses, page, pageSize: 3);

		ViewBag.Pager = ServiceUnitOfWork.PagerService;

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
				UserName = _userManager.GetUserName(User)!
			},
			ProductReviewResponses = pagedProductReviewResponses
		};

		// Use ProductDetailsVM to combine CartItemAddRequestVM and ProductReviewVM
		var productDetailsVM = new ProductDetailsVM()
		{
			CartItemAddRequestVM = cartItemAddRequestVM,
			ProductReviewVM = productReviewVM
		};

		return View(productDetailsVM);
	}

	[HttpPost]
	[Authorize]
	[Route("{id?}")]
	[ActionName(nameof(Details))]
	public async Task<IActionResult> AddToCart(CartItemAddRequestVM cartItemAddRequestVM, CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			await _cartFacade.AddToCartAsync(cartItemAddRequestVM.CartItemAddRequest, this);
			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state");
		ProductResponse? productResponse = await ServiceUnitOfWork.ProductService
			.GetByIdAsync(cartItemAddRequestVM.ProductResponse.Id, cancellationToken: cancellationToken,
			includes: [nameof(ProductResponse.Category), nameof(ProductResponse.ProductImages)]);

		cartItemAddRequestVM.ProductResponse = productResponse!;

		return View(cartItemAddRequestVM);
	}

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
			Logger.LogInformation($"Added review for product: {productReviewAddRequest.ProductId}");
		}
		else
			ErrorMessage = "Please select your rating";

		return RedirectToAction(nameof(Details), new { id = productReviewAddRequest.ProductId });
	}

	public IActionResult Privacy()
	{
		return View();
	}

	[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
	public IActionResult Error()
	{
		return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
	}
}
