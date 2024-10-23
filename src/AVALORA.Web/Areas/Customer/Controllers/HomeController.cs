using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Helpers;
using AVALORA.Web.BaseController;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace AVALORA.Web.Areas.Customer.Controllers;

[Area(SD.ROLE_CUSTOMER)]
[Route("[controller]/[action]")]
public class HomeController : BaseController<HomeController>
{
    public HomeController()
    {
    }

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
