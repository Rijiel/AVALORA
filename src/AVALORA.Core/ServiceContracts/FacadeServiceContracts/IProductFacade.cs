using AVALORA.Core.Dto.ProductDtos;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AVALORA.Core.ServiceContracts.FacadeServiceContracts;

public interface IProductFacade
{
    /// <summary>
    /// Creates a new product and product image asynchronously.
    /// </summary>
    /// <param name="productAddRequest">The request object containing the product details to be added.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task CreateProductAsync(ProductAddRequest? productAddRequest);

    /// <summary>
    /// Updates a product and product images asynchronously.
    /// </summary>
    /// <param name="productUpdateRequest">The product update request.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task UpdateProductAsync(ProductUpdateRequest? productUpdateRequest);

    /// <summary>
    /// Deletes a product and its product images asynchronously.
    /// </summary>
    /// <param name="id">The ID of the product to be deleted.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task DeleteProductAsync(int? id);

	/// <summary>
	/// Gets a list of product categories asynchronously.
	/// </summary>
	/// <param name="cancellationToken">A cancellation token that can be used by 
	/// other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation.</returns>
	Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync(CancellationToken cancellationToken = default);

	/// <summary>
	/// Gets a count of product images for a given product asynchronously.
	/// </summary>
	/// <param name="productId">The ID of the product.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by 
	/// other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation. 
	/// Returns the count of product images.</returns>
	Task<int> GetProductImageCount(int? productId, CancellationToken cancellationToken = default);
}