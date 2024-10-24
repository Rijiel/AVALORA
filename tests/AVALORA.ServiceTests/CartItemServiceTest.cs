using AutoFixture;
using AutoMapper;
using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Moq;

namespace AVALORA.ServiceTests;

public class CartItemServiceTest
{
    private readonly Fixture _fixture;
    private readonly IMapper _mapper;

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICartItemRepository> _cartItemRepositoryMock;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICartItemRepository _cartItemRepository;
    private readonly ICartItemService _cartItemService;

    public CartItemServiceTest()
    {
        _fixture = new Fixture();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _cartItemRepositoryMock = new Mock<ICartItemRepository>();

        _unitOfWork = _unitOfWorkMock.Object;
        _cartItemRepository = _cartItemRepositoryMock.Object;

        _cartItemService = new CartItemService(_cartItemRepository, _mapper, _unitOfWork);
    }

    [Fact]
    public void GetTotalPrice_GivenNullCartItems_ShouldReturnZero()
    {
        // Arrange
        IEnumerable<CartItemResponse>? cartItemResponses = null;

        // Act
        double result = _cartItemService.GetTotalPrice(cartItemResponses);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetTotalPrice_GivenCartItemsWithZeroPrices_ShouldReturnTotalOfZero()
    {
        // Arrange
        var product = new Product() { Price = 0 };
        var cartItemResponses = Enumerable.Range(0, 5)
            .Select(i => _fixture.Build<CartItemResponse>()
            .With(p => p.Product, product).Create());

        // Act
        double result = _cartItemService.GetTotalPrice(cartItemResponses);

        // Assert
        result.Should().Be(0);
    }

    [Fact]
    public void GetTotalPrice_GivenValidCartItemsWithPrices_ShouldReturnTotal()
    {
        // Arrange
        var cartItemResponses = new List<CartItemResponse>()
        {
            _fixture.Build<CartItemResponse>()
            .With(p => p.Product, new Product() {Price = 5})
            .With(p => p.Count, 1).Create(),
            _fixture.Build<CartItemResponse>()
            .With(p => p.Product, new Product() {Price = 10})
            .With(p => p.Count, 3).Create(),
            _fixture.Build<CartItemResponse>()
            .With(p => p.Product, new Product() {Price = 8.99})
            .With(p => p.Count, 12).Create(),
        };

        double expected = 142.88;

        // Act
        double result = _cartItemService.GetTotalPrice(cartItemResponses);

        // Assert
        result.Should().Be(expected);
    }
}

