using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.Areas.User.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AVALORA.Web.Filters.ResultFIlters;

/// <summary>
/// A custom result filter for the HomeController, implementing the IAsyncResultFilter interface.
/// This filter is responsible for handling the result of the HomeController's upsert actions.
/// </summary>
public class HomeControllerResultFilter : IAsyncResultFilter
{
	private readonly IProductFacade _productFacade;

	public HomeControllerResultFilter(IProductFacade productFacade)
	{
		_productFacade = productFacade;
	}

	public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		if (!context.ModelState.IsValid)
		{
			var controller = (HomeController)context.Controller;
			var actionArguments = (IDictionary<string, object>?) context.HttpContext.Items["ActionArguments"];

			if (actionArguments != null)
			{
				var cartItemAddRequestVM = (CartItemAddRequestVM?) actionArguments?
					.FirstOrDefault(r => r.Value is not CancellationToken).Value;
				int id = cartItemAddRequestVM?.CartItemAddRequest?.ProductId ?? 0;
				var cancellationToken = context.HttpContext.RequestAborted;
				
				var productDetailsVM = await _productFacade.GetProductDetailsVMAsync(controller, id, 1, cancellationToken);

				context.Result = controller.View(productDetailsVM);
			} 
		}

		await next();
	}
}
