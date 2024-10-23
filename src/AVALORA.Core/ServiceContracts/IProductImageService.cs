using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using Microsoft.AspNetCore.Http;

namespace AVALORA.Core.ServiceContracts;

public interface IProductImageService : IGenericService<ProductImage, ProductImageAddRequest, ProductImageUpdateRequest, ProductImageResponse>
{
	/// <summary>
	/// Creates product images asynchronously, uploading and storing images in the webroot, 
	/// and persisting metadata in the database.
	/// </summary>
	/// <param name="productId">ID of the product</param>
	/// <param name="imageFiles">Collection of image files to upload.</param>
	/// <returns>List of ProductImageResponse objects from added files.</returns>
	Task<List<ProductImageResponse>> CreateImagesAsync(int? productId, IEnumerable<IFormFile>? imageFiles);

	/// <summary>
	/// Deletes a product image asynchronously in the database and directory.
	/// </summary>
	/// <param name="id">The ID of the product image to delete.</param>
	/// <returns>A task representing the deletion operation.</returns>
	Task DeleteImageAsync(int? id);

	/// <summary>
	/// Deletes all product images associated with a product asynchronously in the database and directory.
	/// </summary>
	/// <param name="productId">The ID of the product to delete images for.</param>
	/// <returns>A task representing the deletion operation.</returns>
	Task DeleteAllImagesAsync(int? productId);
}

