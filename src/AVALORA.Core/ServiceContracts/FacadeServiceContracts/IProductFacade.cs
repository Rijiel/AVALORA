﻿using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.ProductDtos;
using Microsoft.AspNetCore.Mvc;
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

	/// <summary>
	/// Filters a list of product responses based on the provided category, color, and search parameters.
	/// </summary>
	/// <param name="productResponses">The list of product responses to filter.</param>
	/// <param name="category">The category to filter by (optional).</param>
	/// <param name="color">The color to filter by (optional).</param>
	/// <param name="search">The search term to filter by (optional).</param>
	/// <returns>A list of filtered product responses.</returns>
	List<ProductResponse> GetFilteredProducts(List<ProductResponse> productResponses, string? category = null,
		string? color = null, string? search = null);

	/// <summary>
	/// Retrieves a ProductDetailsVM instance asynchronously based on the provided product ID.
	/// </summary>
	/// <param name="controller">The controller context.</param>
	/// <param name="id">The ID of the product to retrieve details for.</param>
	/// <param name="page">The page number for pagination.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive 
	/// notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation, containing the ProductDetailsVM instance.</returns>
	Task<ProductDetailsVM> GetProductDetailsVMAsync(Controller controller, int? id, int page,
		CancellationToken cancellationToken = default);
}