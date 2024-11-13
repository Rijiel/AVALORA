using AutoFixture;
using AutoMapper;
using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Moq;
using System.IO;

namespace AVALORA.ServiceTests;

public class ProductImageServiceTest
{
	private readonly Fixture _fixture;
	private readonly IMapper _mapper;

	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<IProductImageRepository> _productImageRepositoryMock;
	private readonly Mock<IWebHostEnvironment> _webHostEnvironmentMock;

	private readonly IUnitOfWork _unitOfWork;
	private readonly IProductImageRepository _productImageRepository;
	private readonly IProductImageService _productImageService;
	private readonly IWebHostEnvironment _webHostEnvironment;

	public ProductImageServiceTest()
	{
		_fixture = new Fixture();
		_fixture.Customize<Category?>(cfg => cfg.FromFactory(() => null));
		_mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_productImageRepositoryMock = new Mock<IProductImageRepository>();
		_webHostEnvironmentMock = new Mock<IWebHostEnvironment>();
		_webHostEnvironment = _webHostEnvironmentMock.Object;

		_unitOfWork = _unitOfWorkMock.Object;
		_productImageRepository = _productImageRepositoryMock.Object;

		_productImageService = new ProductImageService(_productImageRepository, _mapper, _unitOfWork, _webHostEnvironment);
	}

	#region CreateImagesAsync
	[Fact]
	public async Task CreateImagesAsync_GivenNullProductId_ShouldThrowArgumentNullException()
	{
		// Arrange
		int? productId = null;
		var fileMock = new Mock<IFormFile>();
		fileMock.Setup(f => f.FileName).Returns($"test.jpg");
		var imageFiles = new List<IFormFile>() { fileMock.Object };

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.CreateImagesAsync(productId, imageFiles);
		};

		// Assert
		await result.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task CreateImagesAsync_GivenNullImageFiles_ShouldThrowArgumentNullException()
	{
		// Arrange
		var productId = _fixture.Create<int>();
		IEnumerable<IFormFile>? imageFiles = null;

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.CreateImagesAsync(productId, imageFiles);
		};

		// Assert
		await result.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task CreateImagesAsync_GivenEmptyImageFileList_ShouldThrowArgumentNullException()
	{
		// Arrange
		var productId = _fixture.Create<int>();
		IEnumerable<IFormFile>? imageFiles = [];

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.CreateImagesAsync(productId, imageFiles);
		};

		// Assert
		await result.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task CreateImagesAsync_GivenInvalidProductId_ShouldThrowKeyNotFoundException()
	{
		// Arrange
		var productId = _fixture.Create<int>();

		var fileMock = new Mock<IFormFile>();
		fileMock.Setup(f => f.FileName).Returns($"test.jpg");

		var imageFiles = new List<IFormFile>() { fileMock.Object };

		Product? product = null;
		_unitOfWorkMock.Setup(x => x.Products.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>()))
			.ReturnsAsync(product);

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.CreateImagesAsync(productId, imageFiles);
		};

		// Assert
		await result.Should().ThrowAsync<KeyNotFoundException>();
	}

	[Fact]
	public async Task CreateImagesAsync_GivenInvalidImageFileExtension_ShouldThrowArgumentException()
	{
		// Arrange
		var productId = _fixture.Create<int>();

		var fileMock = new Mock<IFormFile>();
		fileMock.Setup(f => f.FileName).Returns("test.txt");
		fileMock.Setup(f => f.ContentType).Returns("text/plain");
		fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
			.Callback<Stream, CancellationToken>((stream, token) => { });

		var imageFiles = new List<IFormFile>() { fileMock.Object };

		var product = _fixture.Build<Product>()
			.With(p => p.ProductImages, null as List<ProductImage>)
			.With(p => p.ProductReviews, null as List<ProductReview>).Create();
		_unitOfWorkMock.Setup(x => x.Products.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>()))
			.ReturnsAsync(product);

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.CreateImagesAsync(productId, imageFiles);
		};

		// Assert
		await result.Should().ThrowAsync<ArgumentException>();
	}

	[Fact]
	public async Task CreateImagesAsync_GivenValidArguments_ShouldReturnProductImageResponseListWithPath_FileShouldExist()
	{
		// Arrange
		var productId = _fixture.Create<int>();

		var fileMock = new Mock<IFormFile>();
		fileMock.Setup(f => f.FileName).Returns("test.jpg");
		fileMock.Setup(f => f.ContentType).Returns("image/jpeg");
		fileMock.Setup(f => f.CopyToAsync(It.IsAny<Stream>(), It.IsAny<CancellationToken>()))
			.Callback<Stream, CancellationToken>((stream, token) => { });

		var imageFiles = new List<IFormFile> { fileMock.Object };

		_webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(Path.GetTempPath());

		var product = _fixture.Build<Product>()
			.With(p => p.ProductImages, null as List<ProductImage>)
			.With(p => p.ProductReviews, null as List<ProductReview>).Create();
		_unitOfWorkMock.Setup(x => x.Products.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>()))
			.ReturnsAsync(product);

		// Act
		List<ProductImageResponse> result = await _productImageService.CreateImagesAsync(productId, imageFiles);

		// Assert
		result.Should().BeOfType<List<ProductImageResponse>>();
		result.Count.Should().Be(imageFiles.Count);
		result.Select(r => r.Path).Should().NotBeNullOrEmpty();
		_productImageRepositoryMock.Verify(x => x.Add(It.IsAny<ProductImage>()), Times.Exactly(imageFiles.Count));
		var filePath = Path.Combine(_webHostEnvironment.WebRootPath, result[0].Path.TrimStart('\\'));
		Assert.True(File.Exists(filePath));
	}
	#endregion

	#region DeleteImageAsync
	[Fact]
	public async Task DeleteImageAsync_GivenNullId_ShouldThrowArgumentNullException()
	{
		// Arrange
		int? id = null;

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.DeleteImageAsync(id);
		};

		// Assert
		await result.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task DeleteImageAsync_GivenInvalidId_ShouldThrowKeyNotFoundException()
	{
		// Arrange
		int? id = _fixture.Create<int>();

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.DeleteImageAsync(id);
		};

		// Assert
		await result.Should().ThrowAsync<KeyNotFoundException>();
	}

	[Fact]
	public async Task DeleteImageAsync_GivenValidId_ShouldCallRemoveOnce()
	{
		// Arrange
		int? id = _fixture.Create<int>();
		ProductImage productImage = _fixture.Build<ProductImage>()
			.With(p => p.Product, null as Product)
			.With(p => p.Path, "test.jpg").Create();

		_webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(Path.GetTempPath());
		_productImageRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<bool>(), It.IsAny<string[]>()))
			.ReturnsAsync(productImage);

		// Act
		await _productImageService.DeleteImageAsync(id);

		// Assert
		_productImageRepositoryMock.Verify(x => x.Remove(It.IsAny<ProductImage>()), Times.Once);
	}
	#endregion

	#region DeleteAllImagesAsync
	[Fact]
	public async Task DeleteAllImagesAsync_GivenNullProductId_ShouldThrowArgumentNullException()
	{
		// Arrange
		int? productId = null;

		// Act
		Func<Task> result = async () =>
		{
			await _productImageService.DeleteAllImagesAsync(productId);
		};

		// Assert
		await result.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task DeleteAllImagesAsync_GivenValidProductId_ShouldCallRemoveRangeOnce()
	{
		// Arrange
		int? productId = _fixture.Create<int>();
		_webHostEnvironmentMock.Setup(w => w.WebRootPath).Returns(Path.GetTempPath());

		// Act
		await _productImageService.DeleteAllImagesAsync(productId);

		// Assert
		_productImageRepositoryMock.Verify(x => x.RemoveRange(It.IsAny<IEnumerable<ProductImage>>()), Times.Once);
	}
	#endregion
}

