using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Enums;
using AVALORA.Web.Areas.User.Controllers;
using AVALORA.Web.BaseController;
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
    [HttpGet]
	[Breadcrumb("Categories", FromAction = nameof(Index), FromController = typeof(HomeController), AreaName = nameof(Role.Admin))]
	public async Task<IActionResult> Index(CancellationToken cancellationToken)
	{
		// Provide list of category responses for editing
		var categoriesVM = new CategoriesVM()
		{
			CategoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync(cancellationToken: cancellationToken)
		};

		// Setup breadcrumb
		var breadCrumbNode = new MvcBreadcrumbNode(nameof(Index), "Categories", "Categories", areaName: Role.Admin.ToString());
		var breadCrumbNode1 = new MvcBreadcrumbNode(nameof(Index), "Categories", "Create", areaName: Role.Admin.ToString())
		{
			OverwriteTitleOnExactMatch = true,
			Parent = breadCrumbNode
		};
		ViewData["BreadcrumbNode"] = breadCrumbNode1;

		return View(categoriesVM);
	}

	[HttpPost]
	[ActionName(nameof(Index))]
	public async Task<IActionResult> Create(CategoriesVM categoriesVM, CancellationToken cancellationToken)
	{
		if (ModelState.IsValid)
		{
			// Persist new category to database
			await ServiceUnitOfWork.CategoryService.AddAsync(categoriesVM.CategoryAddRequest);
			SuccessMessage = "Category created successfully";

			return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");

		// Re-populate category list for re-display on validation failure
		categoriesVM.CategoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync(cancellationToken: cancellationToken);
		return View(categoriesVM);
	}

	[HttpGet]
	[Route("{id?}")]
	public async Task<IActionResult> Edit(int? id, CancellationToken cancellationToken)
	{
		CategoryResponse? categoryResponse = await ServiceUnitOfWork.CategoryService.GetByIdAsync(id, cancellationToken: cancellationToken);
		if (categoryResponse == null)
		{
			Logger.LogError($"Category with id {id} not found.");
			return NotFound("Category not found.");
		}

		// Setup breadcrumb
		var breadCrumbNode = new MvcBreadcrumbNode(nameof(Index), "Categories", "Categories", areaName: Role.Admin.ToString());
		var breadCrumbNode1 = new MvcBreadcrumbNode(nameof(Edit), "Categories", "Edit", areaName: Role.Admin.ToString())
		{
			OverwriteTitleOnExactMatch = true,
			Parent = breadCrumbNode
		};
		var breadCrumbNode2 = new MvcBreadcrumbNode(nameof(Edit), "Categories", id.ToString(), areaName: Role.Admin.ToString())
		{
			OverwriteTitleOnExactMatch = true,
			Parent = breadCrumbNode1
		};

		ViewData["BreadcrumbNode"] = breadCrumbNode2;
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
			SuccessMessage = "Category updated successfully";

            return RedirectToAction(nameof(Index));
		}

		Logger.LogWarning("Invalid model state. Product not created.");
		return View(updateRequest);
	}

	#region API CALLS
	[HttpDelete]
	[Route("{id?}")]
	public async Task<JsonResult> Delete(int? id, CancellationToken cancellationToken)
	{
		try
		{
			// Disable deletion if category is associated with any product
			if (await ServiceUnitOfWork.ProductService.GetAsync(x => x.CategoryId == id, cancellationToken: cancellationToken) == null)
			{
				await ServiceUnitOfWork.CategoryService.RemoveAsync(id);

				SuccessMessage = "Category deleted successfully.";
				Logger.LogInformation("Category deleted successfully.");

				// Enable client-side redirect after deletion
				return Json(new { success = true, redirectUrl = Url.Action(nameof(Index)) });
			}

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
