using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using AVALORA.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading;

namespace AVALORA.Web.Filters.ResultFIlters;

/// <summary>
/// A custom result filter for the CategoriesController, implementing the IAsyncResultFilter interface.
/// This filter is responsible for handling the result of the CategoriesController's upsert actions.
/// </summary>
public class CategoriesControllerResultFilter : IAsyncResultFilter
{
	private readonly IServiceUnitOfWork _serviceUnitOfWork;

	public CategoriesControllerResultFilter(IServiceUnitOfWork serviceUnitOfWork)
	{
		_serviceUnitOfWork = serviceUnitOfWork;
	}

	public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		if (!context.ModelState.IsValid)
		{
			var controller = (CategoriesController)context.Controller;
			CategoriesVM categoriesVM = controller.CategoriesVM;
			var cancellationToken = context.HttpContext.RequestAborted;

			categoriesVM.CategoryResponses = await _serviceUnitOfWork.CategoryService
				.GetAllAsync(cancellationToken: cancellationToken);

			context.Result = controller.View(categoriesVM);
		}

		await next();
	}
}
