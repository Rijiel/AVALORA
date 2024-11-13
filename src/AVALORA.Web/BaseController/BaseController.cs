using AutoMapper;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Mvc;

namespace AVALORA.Web.BaseController;

/// <summary>
/// Base controller providing centralized access to common dependencies.
/// Enables consistency and reduces boilerplate code in derived controllers.
/// </summary>
/// <typeparam name="TController">Type of the derived controller.</typeparam>
public class BaseController<TController> : Controller where TController : BaseController<TController>
{
    /// <summary>
    /// Gets or sets the success message to be displayed via Toastr.
    /// </summary>
	[TempData]
	public string? SuccessMessage { get; set; }

    /// <summary>
    /// Gets or sets the error message to be displayed via Toastr.
    /// </summary>
	[TempData]
	public string? ErrorMessage { get; set; }

	private ILogger<TController>? _logger;
    private IMapper? _mapper;
    private IServiceUnitOfWork? _serviceUnitOfWork;

    protected ILogger<TController> Logger
        => _logger ??= HttpContext.RequestServices.GetRequiredService<ILogger<TController>>();

    protected IMapper Mapper
        => _mapper ??= HttpContext.RequestServices.GetRequiredService<IMapper>();

    /// <summary>
    /// Represents a unit of work for the service layer, encapsulating access to various services.
    /// </summary>
    protected IServiceUnitOfWork ServiceUnitOfWork
        => _serviceUnitOfWork ??= HttpContext.RequestServices.GetRequiredService<IServiceUnitOfWork>();
}
