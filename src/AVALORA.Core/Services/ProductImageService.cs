using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.ServiceContracts;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Net;

namespace AVALORA.Core.Services;

public class ProductImageService : GenericService<ProductImage, ProductImageAddRequest, ProductImageUpdateRequest, ProductImageResponse>, IProductImageService
{
	private readonly IWebHostEnvironment _webHostEnvironment;

	public ProductImageService(IProductImageRepository repository, IMapper mapper, IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment) : base(repository, mapper, unitOfWork)
	{
		_webHostEnvironment = webHostEnvironment;
	}

	public async Task<List<ProductImageResponse>> CreateImagesAsync(int? productId, IEnumerable<IFormFile>? imageFiles)
	{
		ArgumentNullException.ThrowIfNull(productId);
		ArgumentNullException.ThrowIfNull(imageFiles);

		if (!imageFiles.Any())
			throw new ArgumentNullException("", "At least one image file must be provided.");

		// Verify that the Product exists
		var product = await UnitOfWork.Products.GetByIdAsync(productId)
			?? throw new KeyNotFoundException($"Product with id {productId} not found.");

		List<ProductImageResponse> productImageResponseList = [];

		foreach (var imageFile in imageFiles)
		{
			// Verify that file is of type image
			if (!imageFile.ContentType.StartsWith("image"))
				throw new ArgumentException("Only image files are allowed.");

			// Generate unique file name and path
			string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string productPath = $@"images\products\{productId}";
			string completePath = Path.Combine(wwwRootPath, productPath);

			// Create directory if does not exist.
			if (!Directory.Exists(completePath))
				Directory.CreateDirectory(completePath);

			using var fileStream = new FileStream(Path.Combine(completePath, fileName), FileMode.Create);
			imageFile.CopyTo(fileStream);

			// Persist product image in the database.
			var productImageAddRequest = new ProductImageAddRequest()
			{
				Path = $@"\{productPath}\{fileName}",
				ProductId = productId.Value
			};

			ProductImageResponse productImageResponse = await AddAsync(productImageAddRequest);
			productImageResponseList.Add(productImageResponse);
		}

		return productImageResponseList;
	}

	public async Task DeleteImageAsync(int? id)
	{
		ArgumentNullException.ThrowIfNull(id);

		ProductImageResponse? productImageResponse = await GetByIdAsync(id)
			?? throw new KeyNotFoundException("Product image not found.");

		// Check if image file exists
		if (!String.IsNullOrEmpty(productImageResponse.Path))
		{
			string wwwRootPath = _webHostEnvironment.WebRootPath;
			string completePath = Path.Combine(wwwRootPath, productImageResponse.Path.TrimStart('\\'));

			// Delete file in the directory if exists
			if (System.IO.File.Exists(completePath))
				System.IO.File.Delete(completePath);
		}

		// Remove from database
		await RemoveAsync(id);
	}

	public async Task DeleteAllImagesAsync(int? productId)
	{
		ArgumentNullException.ThrowIfNull(productId);

		string wwwRootPath = _webHostEnvironment.WebRootPath;
		string productPath = $@"images\products\{productId}";
		string completePath = Path.Combine(wwwRootPath, productPath);

		// Delete files in directory and directory itself
		if (Directory.Exists(completePath))
		{
			var filePaths = Directory.GetFiles(completePath);
			foreach (var filePath in filePaths)
				System.IO.File.Delete(filePath);

			Directory.Delete(completePath);
		}

		// Remove from database
		IEnumerable<ProductImageResponse> productImageResponses = await GetAllAsync(p => p.ProductId == productId);

		await RemoveRangeAsync(productImageResponses);
	}
}

