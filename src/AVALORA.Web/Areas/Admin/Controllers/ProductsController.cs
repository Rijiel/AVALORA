using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.Areas.User.Controllers;
using AVALORA.Web.BaseController;
using AVALORA.Web.Filters.ActionFilters;
using AVALORA.Web.Filters.ResultFIlters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;

namespace AVALORA.Web.Areas.Admin.Controllers;

[Area(nameof(Role.Admin))]
[Authorize(Roles = nameof(Role.Admin))]
[Route("[controller]/[action]")]
public class ProductsController : BaseController<ProductsController>
{
	private readonly IProductFacade _productFacade;

	[BindProperty]
	public ProductUpsertVM ProductUpsertVM { get; set; } = null!;

	public ProductsController(IProductFacade productFacade)
	{
		_productFacade = productFacade;
	}

	// GET: /Products/Index
	[Breadcrumb("Products", FromController = typeof(HomeController), FromAction = nameof(HomeController.Index),
		AreaName = nameof(Role.Admin))]
	public IActionResult Index() => View();

	// GET: /Products/Create
	[HttpGet]
	[Breadcrumb("Create", FromAction = nameof(Index))]
	public async Task<IActionResult> Create(CancellationToken cancellationToken)
	{
		// Initialize ProductUpsertVM
		ProductUpsertVM = new ProductUpsertVM()
		{
			Categories = await _productFacade.GetCategoriesSelectListAsync(cancellationToken: cancellationToken)
		};

		return View(ProductUpsertVM);
	}

	// POST: /Products/Create
	[HttpPost]
	[ActionName(nameof(Create))]
	[TypeFilter(typeof(ValidationActionFilter))]
	[TypeFilter(typeof(ProductsControllerResultFilter))]
	public async Task<IActionResult> CreatePOST(CancellationToken cancellationToken)
	{
		var productAddRequest = Mapper.Map<ProductAddRequest>(ProductUpsertVM);
		await _productFacade.CreateProductAsync(productAddRequest);

		SuccessMessage = "Product created successfully.";
		Logger.LogInformation(SuccessMessage);

		return RedirectToAction(nameof(Index));
	}

	// GET: /Products/Edit/{id}
	[HttpGet("{id?}")]
	public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
	{
		// Get product to edit
		ProductResponse? productResponse = await ServiceUnitOfWork.ProductService
			.GetByIdAsync(id, cancellationToken: cancellationToken, includes: nameof(ProductResponse.ProductImages));

		if (productResponse == null)
		{
			Logger.LogWarning("Product {productId} not found.", id);
			return NotFound("Product not found.");
		}

		var productUpsertVM = Mapper.Map<ProductUpsertVM>(productResponse);
		productUpsertVM.Categories = await _productFacade.GetCategoriesSelectListAsync(cancellationToken);

		ServiceUnitOfWork.BreadcrumbService.SetCustomNodes(this, "Products",
			controllerActions: [nameof(Index), nameof(Edit), nameof(Edit)], titles: ["Products", "Edit", id.ToString()!]);

		return View(productUpsertVM);
	}

	// POST: /Products/Edit/{id}
	[HttpPost("{id?}")]
	[TypeFilter(typeof(ValidationActionFilter))]
	[TypeFilter(typeof(ProductsControllerResultFilter))]
	public async Task<IActionResult> Edit(CancellationToken cancellationToken)
	{
		var productUpdateRequest = Mapper.Map<ProductUpdateRequest>(ProductUpsertVM);
		await _productFacade.UpdateProductAsync(productUpdateRequest);

		SuccessMessage = "Product updated successfully.";
		Logger.LogInformation(SuccessMessage);

		return RedirectToAction(nameof(Index));
	}

	#region API CALLS
	// GET: /Products/GetAll
	public async Task<JsonResult> GetAll(CancellationToken cancellationToken)
	{
		List<ProductResponse> productResponses = await ServiceUnitOfWork.ProductService
			.GetAllAsync(includes: [nameof(ProductResponse.Category)], cancellationToken: cancellationToken);

		// Initialize product images count for each product
		foreach (var productResponse in productResponses)
		{
			productResponse.ProductImagesCount = await _productFacade
				.GetProductImageCount(productResponse.Id, cancellationToken);
		}

		Logger.LogInformation("Retrieved all products with product images count.");
		return Json(new { data = productResponses });
	}

	// DELETE: /Products/Delete?id={id}
	[HttpDelete]
	public async Task<JsonResult> Delete(int? id)
	{
		try
		{
			await _productFacade.DeleteProductAsync(id);

			SuccessMessage = "Product deleted successfully.";
			Logger.LogInformation($"Product {id} deleted successfully.");

			return Json(new { success = true });
		}
		catch (Exception e)
		{
			ErrorMessage = e.Message;
			Logger.LogError(e.Message);

			return Json(new { success = false });
		}
	}

	// GET: /Products/DeleteImage/{id}
	[Route("{id?}")]
	public async Task<IActionResult> DeleteImage(int? id, CancellationToken cancellationToken)
	{
		await ServiceUnitOfWork.ProductImageService.RemoveAsync(id);
		SuccessMessage = "Product image deleted successfully.";

		// Get product id from product image
		ProductImageResponse? productImageResponse = await ServiceUnitOfWork.ProductImageService
			.GetByIdAsync(id, cancellationToken: cancellationToken);

		return RedirectToAction(nameof(Edit), new { id = productImageResponse?.ProductId });
	}
	#endregion
}
