using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.Areas.Admin.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading;

namespace AVALORA.Web.Filters.ResultFIlters;

/// <summary>
/// A custom result filter for the ProductsController, implementing the IAsyncResultFilter interface.
/// This filter is responsible for handling the result of the ProductsController's upsert actions.
/// </summary>
public class ProductsControllerResultFilter : IAsyncResultFilter
{
	private readonly IProductFacade _productFacade;

	public ProductsControllerResultFilter(IProductFacade productFacade)
    {
		_productFacade = productFacade;
	}

    public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
			var controller = (ProductsController)context.Controller;
			ProductUpsertVM productUpsertVM = controller.ProductUpsertVM;
			var cancellationToken = context.HttpContext.RequestAborted;

			productUpsertVM.Categories = await _productFacade.GetCategoriesSelectListAsync(cancellationToken);

			context.Result = controller.View(productUpsertVM);			
		}

		await next();
	}
}
