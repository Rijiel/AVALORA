using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.Helpers;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.Admin.Controllers;

[Area(SD.ROLE_ADMIN)]
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
	public async Task<IActionResult> Create()
	{
		ProductUpsertVM = new ProductUpsertVM()
		{
			Categories = await _productFacade.GetCategoriesSelectListAsync()
		};

		return View(ProductUpsertVM);
	}

	[HttpPost]
	[ActionName(nameof(Create))]
	public async Task<IActionResult> CreatePOST()
	{
		if (ModelState.IsValid)
		{
			var productAddRequest = Mapper.Map<ProductAddRequest>(ProductUpsertVM);
			await _productFacade.CreateProductAsync(productAddRequest);
            TempData["Success"] = "Product created successfully.";

            return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");
		ProductUpsertVM.Categories = await _productFacade.GetCategoriesSelectListAsync();
        return View(ProductUpsertVM);
	}

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(int? id)
	{
		ProductResponse? productResponse = 
			await ServiceUnitOfWork.ProductService.GetByIdAsync(id, includes: nameof(ProductResponse.ProductImages));

		if (productResponse == null)
		{
			Logger.LogError($"Product with id {id} not found.");
			return NotFound("Product not found.");
		}

		var productUpsertVM = Mapper.Map<ProductUpsertVM>(productResponse);
		productUpsertVM.Categories = await _productFacade.GetCategoriesSelectListAsync();

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

			TempData["Success"] = "Product updated successfully.";
		}

		Logger.LogWarning("Invalid model state. Product not created.");

		return RedirectToAction(nameof(Index));
	}

	#region API CALLS
	public async Task<JsonResult> GetAll()
	{
		List<ProductResponse> productResponses = await ServiceUnitOfWork.ProductService
			.GetAllAsync(includes: [nameof(ProductResponse.Category)]);

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
			TempData["Success"] = "Product deleted successfully.";

			return Json(new { success = true });
		}
		catch (Exception e)
		{
			TempData["Error"] = e.Message;

			return Json(new { success = false });
		}
	}

	[Route("{id?}")]
	public async Task<IActionResult> DeleteImage(int? id)
	{
		ProductImageResponse? productImageResponse =  await ServiceUnitOfWork.ProductImageService.GetByIdAsync(id);

		await ServiceUnitOfWork.ProductImageService.RemoveAsync(id);
		TempData["Success"] = "Product image deleted successfully.";

		return RedirectToAction(nameof(Edit), new { id = productImageResponse?.ProductId });
	}
	#endregion
}
