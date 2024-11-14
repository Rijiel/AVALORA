using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.Services;
using AVALORA.Web.Areas.User.Controllers;
using AVALORA.Web.BaseController;
using AVALORA.Web.Filters.ActionFilters;
using AVALORA.Web.Filters.ResultFIlters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartBreadcrumbs.Attributes;
using SmartBreadcrumbs.Nodes;

namespace AVALORA.Web.Areas.Admin.Controllers;
[Area(nameof(Role.Admin))]
[Authorize(Roles = nameof(Role.Admin))]
[Route("[controller]/[action]")]
public class CategoriesController : BaseController<CategoriesController>
{
	[BindProperty]
	public CategoriesVM CategoriesVM { get; set; } = null!;

	// GET: /Categories/Index
	[HttpGet]
	[Breadcrumb("Categories", FromAction = nameof(Index), FromController = typeof(HomeController),
		AreaName = nameof(Role.Admin))]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		// Provide list of category responses for editing
		CategoriesVM = new CategoriesVM()
		{
			CategoryResponses = await ServiceUnitOfWork.CategoryService
			.GetAllAsync(cancellationToken: cancellationToken)
		};

		ServiceUnitOfWork.BreadcrumbService.SetCustomNodes(this, "Categories",
			controllerActions: [nameof(Index), nameof(Create)], titles: ["Categories", "Create"]);

		return View(CategoriesVM);
	}

	// POST: /Categories/Index
	[HttpPost]
	[ActionName(nameof(Index))]
	[TypeFilter(typeof(ValidationActionFilter))]
	[TypeFilter(typeof(CategoriesControllerResultFilter))]
	public async Task<IActionResult> Create(CancellationToken cancellationToken)
	{
		// Persist new category to database
		CategoryResponse categoryResponse = await ServiceUnitOfWork.CategoryService
			.AddAsync(CategoriesVM.CategoryAddRequest);

		SuccessMessage = "Category created successfully";
		Logger.LogInformation("Category {categoryId} created successfully.", categoryResponse.Id);

		return RedirectToAction(nameof(Index));
	}

	// GET: /Categories/Edit/{id}
	[HttpGet("{id?}")]
	public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
	{
		// Get category to edit
		CategoryResponse? categoryResponse = await ServiceUnitOfWork.CategoryService
			.GetByIdAsync(id, cancellationToken: cancellationToken);

		if (categoryResponse == null)
		{
			Logger.LogWarning("Category {categoryId} not found.", id);
			return NotFound("Category not found.");
		}

		ServiceUnitOfWork.BreadcrumbService.SetCustomNodes(this, "Categories", controllerActions:
			[nameof(Index), nameof(Edit), nameof(Edit)], titles: ["Categories", "Edit", id.ToString()!]);

		return View(Mapper.Map<CategoryUpdateRequest>(categoryResponse));
	}

	// POST: /Categories/Edit/{id}
	[HttpPost("{id?}")]
	[TypeFilter(typeof(ValidationActionFilter))]
	public async Task<IActionResult> Edit(CategoryUpdateRequest updateRequest)
	{
		// Persist updated category to database
		await ServiceUnitOfWork.CategoryService.UpdateAsync(updateRequest);

		Logger.LogInformation("Category {categoryId} updated successfully.", updateRequest.Id);
		SuccessMessage = "Category updated successfully";

		return RedirectToAction(nameof(Index));
	}

	#region API CALLS
	// DELETE: /Categories/Delete/{id}
	[HttpDelete("{id?}")]
	public async Task<JsonResult> Delete(int? id, CancellationToken cancellationToken)
	{
		try
		{
			// Disable deletion if category is associated with any product
			if (await ServiceUnitOfWork.ProductService.GetAsync(x => x.CategoryId == id,
				cancellationToken: cancellationToken) == null)
			{
				await ServiceUnitOfWork.CategoryService.RemoveAsync(id);

				SuccessMessage = "Category deleted successfully.";
				Logger.LogInformation("Category {categoryId} deleted successfully.", id);

				// Enable client-side redirect after deletion
				return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
			}

			Logger.LogWarning("Category {categoryId} is associated with one or more products.", id);

			throw new Exception("Category is associated with one or more products.");
		}
		catch (Exception e)
		{
			ErrorMessage = e.Message;
			Logger.LogError(e.Message);

			return Json(new { success = false });
		}
	}
	#endregion
}
