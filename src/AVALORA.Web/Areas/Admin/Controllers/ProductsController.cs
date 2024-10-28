using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.Admin.Controllers;

[Area(nameof(Role.Admin))]
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

	public IActionResult Index() => View();

	[HttpGet]
	public async Task<IActionResult> Create(CancellationToken cancellationToken)
	{
		ProductUpsertVM = new ProductUpsertVM()
		{
			Categories = await _productFacade.GetCategoriesSelectListAsync(cancellationToken: cancellationToken)
		};

		return View(ProductUpsertVM);
	}

	[HttpPost]
	[ActionName(nameof(Create))]
	public async Task<IActionResult> CreatePOST(CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			var productAddRequest = Mapper.Map<ProductAddRequest>(ProductUpsertVM);
			await _productFacade.CreateProductAsync(productAddRequest);
			SuccessMessage = "Product created successfully.";

			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");
		ProductUpsertVM.Categories = await _productFacade.GetCategoriesSelectListAsync(cancellationToken: cancellationToken);
		return View(ProductUpsertVM);
	}

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
	{
		ProductResponse? productResponse =
			await ServiceUnitOfWork.ProductService.GetByIdAsync(id, cancellationToken: cancellationToken, includes: nameof(ProductResponse.ProductImages));

		if (productResponse == null)
		{
			Logger.LogError($"Product with id {id} not found.");
			return NotFound("Product not found.");
		}

		var productUpsertVM = Mapper.Map<ProductUpsertVM>(productResponse);
		productUpsertVM.Categories = await _productFacade.GetCategoriesSelectListAsync(cancellationToken);

		return View(productUpsertVM);
	}

	[HttpPost]
	[Route("{id?}")]
	public async Task<IActionResult> Edit()
	{
		if (ModelState.IsValid)
		{
			var productUpdateRequest = Mapper.Map<ProductUpdateRequest>(ProductUpsertVM);
			await _productFacade.UpdateProductAsync(productUpdateRequest);

			SuccessMessage = "Product updated successfully.";
			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");

		return View(ProductUpsertVM);
	}

	#region API CALLS
	public async Task<JsonResult> GetAll(CancellationToken cancellationToken)
	{
		List<ProductResponse> productResponses = await ServiceUnitOfWork.ProductService
			.GetAllAsync(cancellationToken: cancellationToken, includes: [nameof(ProductResponse.Category)]);

		foreach (var productResponse in productResponses)
			productResponse.ProductImagesCount = await _productFacade.GetProductImageCount(productResponse.Id);

		Logger.LogInformation("Retrieved all products with product images count.");
		return Json(new { data = productResponses });
	}

	[HttpDelete]
	public async Task<JsonResult> Delete(int? id)
	{
		try
		{
			await _productFacade.DeleteProductAsync(id);
			SuccessMessage = "Product deleted successfully.";

			return Json(new { success = true });
		}
		catch (Exception e)
		{
			TempData["Error"] = e.Message;

			return Json(new { success = false });
		}
	}

	[Route("{id?}")]
	public async Task<IActionResult> DeleteImage(int? id, CancellationToken cancellationToken)
	{
		ProductImageResponse? productImageResponse = await ServiceUnitOfWork.ProductImageService
			.GetByIdAsync(id, cancellationToken: cancellationToken);

		await ServiceUnitOfWork.ProductImageService.RemoveAsync(id);
		SuccessMessage = "Product image deleted successfully.";

		return RedirectToAction(nameof(Edit), new { id = productImageResponse?.ProductId });
	}
	#endregion
}
