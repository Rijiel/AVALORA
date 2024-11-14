using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using AVALORA.Web.Areas.User.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Threading;

namespace AVALORA.Web.Filters.ResultFIlters;

/// <summary>
/// A custom result filter for the CartController, implementing the IAsyncResultFilter interface.
/// This filter is responsible for handling the result of the CartController's upsert actions.
/// </summary>
public class CartControllerResultFilter : IAsyncResultFilter
{
	private readonly ICartFacade _cartFacade;

	public CartControllerResultFilter(ICartFacade cartFacade)
	{
		_cartFacade = cartFacade;
	}

	public async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
	{
		if (!context.ModelState.IsValid)
		{
			CartController cartController = (CartController)context.Controller;
			CheckoutVM checkoutVM = cartController.CheckoutVM;
			CancellationToken cancellationToken = context.HttpContext.RequestAborted;

			checkoutVM.CartItemResponses = await _cartFacade
				.GetCurrentUserCartItemsAsync(cancellationToken: cancellationToken);

			context.Result = cartController.View(checkoutVM);
		}

		await next();
	}
}
