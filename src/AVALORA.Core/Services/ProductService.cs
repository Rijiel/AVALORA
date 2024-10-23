using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class ProductService : GenericService<Product, ProductAddRequest, ProductUpdateRequest, ProductResponse>, IProductService
{
    public ProductService(IProductRepository repository, IMapper mapper, IUnitOfWork unitOfWork) : base(repository, mapper, unitOfWork)
	{        
    }
}

