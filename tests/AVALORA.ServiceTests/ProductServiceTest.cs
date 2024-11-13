using AutoFixture;
using AutoMapper;
using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Moq;

namespace AVALORA.ServiceTests;

public class ProductServiceTest
{
	private readonly Fixture _fixture;
	private readonly IMapper _mapper;

	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<IProductRepository> _productRepositoryMock;

	private readonly IUnitOfWork _unitOfWork;
	private readonly IProductRepository _productRepository;
	private readonly IProductService _productService;

	public ProductServiceTest()
	{
		_fixture = new Fixture();
		_mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_productRepositoryMock = new Mock<IProductRepository>();

		_unitOfWork = _unitOfWorkMock.Object;
		_productRepository = _productRepositoryMock.Object;

		_productService = new ProductService(_productRepository, _mapper, _unitOfWork);
	}

	[Fact]
	public async Task GetTotalRatingAsync_GivenNullId_ShouldReturnZero()
	{
		// Arrange
		int? id = null;

		// Act
		var result = await _productService.GetTotalRatingAsync(id);

		// Assert
		result.Should().Be(0);
	}

	[Fact]
	public async Task GetTotalRatingAsync_GivenInvalidId_ShouldReturnZero()
	{
		// Arrange
		int id = _fixture.Create<int>();

		// Act
		var result = await _productService.GetTotalRatingAsync(id);

		// Assert
		result.Should().Be(0);
	}

	[Fact]
	public async Task GetTotalRatingAsync_GivenValidId_ShouldReturnTotalRating()
	{
		// Arrange
		int id = _fixture.Create<int>();
		var productReviews = new List<ProductReview>()
		{
			_fixture.Build<ProductReview>().With(p => p.Rating, 5)
			.With(p => p.Product, null as Product).Create(),
			_fixture.Build<ProductReview>().With(p => p.Rating, 3)
			.With(p => p.Product, null as Product).Create(),
			_fixture.Build<ProductReview>().With(p => p.Rating, 1)
			.With(p => p.Product, null as Product).Create(),
		};

		var product = _fixture.Build<Product>()
			.With(p => p.ProductImages, null as List<ProductImage>)
			.With(p => p.ProductReviews, null as List<ProductReview>)
			.With(p => p.ProductReviews, productReviews).Create();

		decimal expected = (decimal)productReviews.Average(r => r.Rating);

		_productRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>()))
			.ReturnsAsync(product);

		// Act
		var result = await _productService.GetTotalRatingAsync(id);

		// Assert
		result.Should().Be(expected);
	}
}