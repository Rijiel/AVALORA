using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace AVALORA.Core.Services.FacadeServices;

public class ProductFacade : BaseFacade<ProductFacade>, IProductFacade
{
	public ProductFacade(IServiceProvider serviceProvider) : base(serviceProvider)
	{
	}

	public async Task CreateProductAsync(ProductAddRequest? productAddRequest)
	{
		ProductResponse productResponse = await ServiceUnitOfWork.ProductService.AddAsync(productAddRequest);
		ProductUpdateRequest productUpdateRequest = Mapper.Map<ProductUpdateRequest>(productResponse);

		// Check if there are any image files in the request
		if (productAddRequest!.ImageFiles?.Count() > 0)
		{
			List<ProductImageResponse> productImageResponses =
				await ServiceUnitOfWork.ProductImageService.CreateImagesAsync(productResponse.Id, productAddRequest.ImageFiles);
			productUpdateRequest.ProductImages = Mapper.Map<List<ProductImage>>(productImageResponses);
			Logger.LogInformation($"Created images for product: {productResponse.Id}");
		}

		// Update the product to contain the images after creating them
		await ServiceUnitOfWork.ProductService.UpdateAsync(productUpdateRequest);
		Logger.LogInformation($"Created product: {productResponse.Id}");
	}

	public async Task UpdateProductAsync(ProductUpdateRequest? productUpdateRequest)
	{
		if (productUpdateRequest != null)
		{
			// Create product images if image files exist
			if (productUpdateRequest.ImageFiles?.Count() > 0)
			{
				List<ProductImageResponse> productImageResponses = await
					ServiceUnitOfWork.ProductImageService.CreateImagesAsync(productUpdateRequest.Id, productUpdateRequest.ImageFiles);
				productUpdateRequest.ProductImages = Mapper.Map<List<ProductImage>>(productImageResponses);
				Logger.LogInformation($"Created images for product: {productUpdateRequest.Id}");
			}

			await ServiceUnitOfWork.ProductService.UpdateAsync(productUpdateRequest);
			Logger.LogInformation($"Updated product: {productUpdateRequest.Id}");
		}
	}

	public async Task DeleteProductAsync(int? id)
	{
		await ServiceUnitOfWork.ProductService.RemoveAsync(id);

		// Delete product images
		await ServiceUnitOfWork.ProductImageService.DeleteAllImagesAsync(id);
		Logger.LogInformation($"Deleted product: {id} and product images");
	}

	public async Task<IEnumerable<SelectListItem>> GetCategoriesSelectListAsync()
	{
		List<CategoryResponse> categoryResponses = await ServiceUnitOfWork.CategoryService.GetAllAsync();
		return categoryResponses.Select(c => new SelectListItem(c.Name, c.Id.ToString()));
	}

	public async Task<int> GetProductImageCount(int? productId)
	{
		int count = 0;
		if (productId != null)
		{
			IEnumerable<ProductImageResponse> productImageResponses =
				await ServiceUnitOfWork.ProductImageService.GetAllAsync(p => p.ProductId == productId);
			count = productImageResponses.Count();
		}

		return count;
	}
}

