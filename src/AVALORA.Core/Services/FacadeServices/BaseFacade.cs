using AutoMapper;
using AVALORA.Core.ServiceContracts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AVALORA.Core.Services.FacadeServices;

public class BaseFacade<TService> where TService : BaseFacade<TService>
{
    private readonly IServiceProvider _serviceProvider;

    private ILogger<TService>? _logger;
    private IMapper? _mapper;
    private IServiceUnitOfWork? _serviceUnitOfWork;

    public BaseFacade(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected ILogger<TService> Logger
        => _logger ??= _serviceProvider.GetRequiredService<ILogger<TService>>();
    protected IMapper Mapper
        => _mapper ??= _serviceProvider.GetRequiredService<IMapper>();
    protected IServiceUnitOfWork ServiceUnitOfWork
        => _serviceUnitOfWork ??= _serviceProvider.GetRequiredService<IServiceUnitOfWork>();
}

