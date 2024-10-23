using AVALORA.Core.Dto.ProductDtos;

namespace AVALORA.Core.ServiceContracts.FacadeServiceContracts;

public interface IProductFacade
{
    /// <summary>
    /// Creates a new product and product image asynchronously.
    /// </summary>
    /// <param name="productAddRequest">The request object containing the product details to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateProductAsync(ProductAddRequest productAddRequest);
}