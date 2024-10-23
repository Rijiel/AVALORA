using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Helpers;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.Areas.Admin.Controllers;

[Area(SD.ROLE_ADMIN)]
[Route("[controller]/[action]")]
public class ProductsController : BaseController<ProductsController>
{
	public IActionResult Index() => View();

	public IActionResult Create()
	{
		return View();
	}

	#region API CALLS
	public async Task<JsonResult> GetAll()
	{
		Logger.LogInformation("Retrieving all products.");
		List<ProductResponse> productResponses = await ServiceUnitOfWork.ProductService
			.GetAllAsync(includes: [nameof(ProductResponse.Category), nameof(ProductResponse.ProductImages)]);

		return Json(new { data = productResponses });
	}
	#endregion
}
