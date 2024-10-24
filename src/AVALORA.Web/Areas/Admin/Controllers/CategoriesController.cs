using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Helpers;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.Admin.Controllers;
[Area(SD.ROLE_ADMIN)]
[Route("[controller]/[action]")]
public class CategoriesController : BaseController<CategoriesController>
{
	[HttpGet]
	public async Task<IActionResult> Index()
	{
		// Provide list of category responses for editing
		var categoriesVM = new CategoriesVM()
		{
			CategoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync()
		};

		return View(categoriesVM);
	}

	[HttpPost]
	[ActionName(nameof(Index))]
	public async Task<IActionResult> Create(CategoriesVM categoriesVM)
	{
		if (ModelState.IsValid)
		{
			// Persist new category to database
			await ServiceUnitOfWork.CategoryService.AddAsync(categoriesVM.CategoryAddRequest);
			TempData["Success"] = "Category created successfully";

			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");

		// Re-populate category list for re-display on validation failure
		categoriesVM.CategoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync();
		return View(categoriesVM);
	}

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(int? id)
	{
		CategoryResponse? categoryResponse = await ServiceUnitOfWork.CategoryService.GetByIdAsync(id);
		if (categoryResponse == null)
		{
			Logger.LogError($"Category with id {id} not found.");
			return NotFound("Category not found.");
		}			

        return View(Mapper.Map<CategoryUpdateRequest>(categoryResponse));
	}

	[HttpPost]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(CategoryUpdateRequest updateRequest)
	{
		if (ModelState.IsValid)
		{
			// Persist updated category to database
			await ServiceUnitOfWork.CategoryService.UpdateAsync(updateRequest);
			TempData["Success"] = "Category updated successfully";

            return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");
		return View(updateRequest);
	}

	#region API CALLS
	[HttpDelete]
	[Route("{id?}")]
	public async Task<JsonResult> Delete(int? id)
	{
		try
		{
			await ServiceUnitOfWork.CategoryService.RemoveAsync(id);

			TempData["Success"] = "Category deleted successfully.";
			Logger.LogInformation("Category deleted successfully.");

			// Enable client-side redirect after deletion
			return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
		}
		catch (Exception e)
		{
			TempData["Error"] = e.Message;
			Logger.LogInformation(e.Message);

			return Json(new { success = false });
		}
	}
	#endregion
}
