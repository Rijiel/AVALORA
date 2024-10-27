using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductReviewDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Core.Services;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AVALORA.Web.Areas.User.Controllers;

[Area(nameof(Role.User))]
[Route("[controller]/[action]")]
public class HomeController : BaseController<HomeController>
{
	private readonly ICartFacade _cartFacade;
	private readonly IHttpContextAccessor _contextAccessor;

	public HomeController(ICartFacade cartFacade, IHttpContextAccessor contextAccessor)
	{
		_cartFacade = cartFacade;
		_contextAccessor = contextAccessor;
	}

	[Route("/")]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		var productResponses = await ServiceUnitOfWork.ProductService
			.GetAllAsync(cancellationToken: cancellationToken, includes: nameof(ProductResponse.ProductImages));

		return View(productResponses);
	}

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken, [FromQuery] int page = 1)
	{
		ProductResponse? productResponse = await ServiceUnitOfWork.ProductService
			.GetByIdAsync(id, cancellationToken: cancellationToken,
			includes: [nameof(ProductResponse.Category), nameof(ProductResponse.ProductImages)]);

		if (productResponse == null)
		{
			Logger.LogWarning("Product not found");
			return NotFound();
		}

		// Prevent negative pages
		const int pageSize = 5;
		if (page < 1)
			page = 1;

		var productReviewResponses = await ServiceUnitOfWork.ProductReviewService.GetAllAsync(p => p.ProductId == productResponse.Id, cancellationToken: cancellationToken);

		int recsCount = productReviewResponses.Count();

		var pager = new Pager(recsCount, page, pageSize);

		int recSkip = (page - 1) * pageSize;

		var data = productReviewResponses
			.Skip(recSkip)
			.Take(pager.PageSize)
			.ToList();

		ViewBag.Pager = pager;

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

		var productReviewVM = new ProductReviewVM()
		{
			ProductReviewAddRequest = new ProductReviewAddRequest()
			{
				ProductId = productResponse.Id,
				DatePosted = DateTime.Now
			},
			ProductReviewResponses = data
		};

		// Use ProductDetailsVM to combine CartItemAddRequestVM and product reviews
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
	public async Task<IActionResult> Details(CartItemAddRequestVM cartItemAddRequestVM, CancellationToken cancellationToken)
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
			SuccessMessage = "Review added, thanks!";
			Logger.LogInformation($"Added review for product: {productReviewAddRequest.ProductId}");
		}

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
