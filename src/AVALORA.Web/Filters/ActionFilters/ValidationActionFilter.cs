using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace AVALORA.Web.Filters.ActionFilters;

/// <summary>
/// A validation action filter that checks the model state before executing an action.
/// </summary>
public class ValidationActionFilter : IActionFilter
{
	public bool OverrideResult { get; set; }

	public ValidationActionFilter(bool overrideResult = false)
	{
		OverrideResult = overrideResult;
	}

	public void OnActionExecuted(ActionExecutedContext context)
	{
	}

	public void OnActionExecuting(ActionExecutingContext context)
	{
		var actionArguments = context.ActionArguments;
		context.HttpContext.Items["ActionArguments"] = actionArguments;

		if (!context.ModelState.IsValid)
		{
			var request = actionArguments?.FirstOrDefault(r => r.Value is not CancellationToken).Value;

			var controller = context.Controller as Controller;

			context.Result = OverrideResult
			? controller?.View()
			: controller?.View(request);
		}
	}
}
