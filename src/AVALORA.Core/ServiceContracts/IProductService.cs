using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ProductDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IProductService : IGenericService<Product, ProductAddRequest, ProductUpdateRequest, ProductResponse>
{
}

