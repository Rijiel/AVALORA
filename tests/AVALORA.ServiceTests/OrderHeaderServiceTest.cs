using AutoFixture;
using AutoMapper;
using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Enums;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Moq;

namespace AVALORA.ServiceTests;

public class OrderHeaderServiceTest
{
	private readonly Fixture _fixture;
	private readonly IMapper _mapper;

	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<IOrderHeaderRepository> _orderHeaderRepositoryMock;

	private readonly IUnitOfWork _unitOfWork;
	private readonly IOrderHeaderRepository _orderHeaderRepository;
	private readonly IOrderHeaderService _orderHeaderService;

	public OrderHeaderServiceTest()
	{
		_fixture = new Fixture();
		_mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_orderHeaderRepositoryMock = new Mock<IOrderHeaderRepository>();

		_unitOfWork = _unitOfWorkMock.Object;
		_orderHeaderRepository = _orderHeaderRepositoryMock.Object;

		_orderHeaderService = new OrderHeaderService(_orderHeaderRepository, _mapper, _unitOfWork);
	}

	#region SetOrderHeaderDefaults
	[Fact]
	public void SetOrderHeaderDefaults_SetsDefaultValuesForOrderHeader()
	{
		// Arrange
		var orderHeaderAddRequest = _fixture.Create<OrderHeaderAddRequest>();

		// Act
		OrderHeaderAddRequest result = _orderHeaderService.SetOrderHeaderDefaults(orderHeaderAddRequest);

		// Assert
		result.OrderStatus.Should().Be(OrderStatus.Pending);
		result.OrderDate.Should().BeOnOrAfter(DateTime.Now.AddDays(-1));
		result.PaymentStatus.Should().Be(PaymentStatus.Pending);
		result.PaymentDueDate.Should().BeOnOrAfter(DateTime.Now.AddDays(-1));
	}
	#endregion

	#region UpdateOrderStatusAsync
	[Fact]
	public async Task UpdateOrderStatusAsync_GivenInvalidId_ShouldThrowKeyNotFoundException()
	{
		// Arrange
		int? id = _fixture.Create<int>();

		// Act
		Func<Task> result = async () =>
		{
			await _orderHeaderService.UpdateOrderStatusAsync(id, It.IsAny<OrderStatus>());
		};

		// Assert
		await result.Should().ThrowAsync<KeyNotFoundException>();
	}

	[Fact]
	public async Task UpdateOrderStatusAsync_GivenOrderStatusWithoutPaymentStatus_ShouldReturnResponseWithUpdatedOrderStatus()
	{
		// Arrange
		int? id = _fixture.Create<int>();
		OrderHeader orderHeader = _fixture.Create<OrderHeader>();
		OrderHeaderUpdateRequest orderHeaderUpdateRequest = _fixture.Create<OrderHeaderUpdateRequest>();

		OrderStatus orderStatus = OrderStatus.Shipped;
		var expected = _mapper.Map<OrderHeaderResponse>(orderHeader);
		expected.OrderStatus = orderStatus;

		_orderHeaderRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(orderHeader);

		// Act
		var result = await _orderHeaderService.UpdateOrderStatusAsync(id, orderStatus);

		// Assert
		result.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.ShippingDate));
		result.ShippingDate.Should().NotBe((default));
	}
	[Fact]
	public async Task UpdateOrderStatusAsync_GivenOrderStatusWithPaymentStatus_ShouldReturnResponseWithUpdatedOrderStatus()
	{
		// Arrange
		int id = _fixture.Create<int>();
		OrderHeader orderHeader = _fixture.Create<OrderHeader>();

		OrderStatus orderStatus = OrderStatus.Approved;
		PaymentStatus paymentStatus = PaymentStatus.Pending;
		var expected = _mapper.Map<OrderHeaderResponse>(orderHeader);
		expected.OrderStatus = orderStatus;
		expected.PaymentStatus = paymentStatus;

		_orderHeaderRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(orderHeader);

		// Act
		var result = await _orderHeaderService.UpdateOrderStatusAsync(id, orderStatus, paymentStatus);

		// Assert
		result.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.ShippingDate));
		result.ShippingDate.Should().NotBe((default));
	}
	#endregion

	#region UpdatePaymentStatusAsync
	[Fact]
	public async Task UpdatePaymentStatusAsync_GivenInvalidId_ShouldThrowKeyNotFoundException()
	{
		// Arrange
		int? id = _fixture.Create<int>();
		string paymentId = _fixture.Create<string>();

		// Act
		Func<Task> result = async () => await _orderHeaderService.UpdatePaymentIdAsync(id, paymentId);

		// Assert
		await result.Should().ThrowAsync<KeyNotFoundException>();
	}

	[Fact]
	public async Task UpdatePaymentStatusAsync_GivenEmptyPaymentId_ShouldThrowArgumentNullException()
	{
		// Arrange
		int? id = _fixture.Create<int>();
		string? paymentId = String.Empty;
		OrderHeader orderHeader = _fixture.Build<OrderHeader>().With(o => o.ApplicationUser, null as ApplicationUser).Create();

		_orderHeaderRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(orderHeader);

		// Act
		Func<Task> result = async () => await _orderHeaderService.UpdatePaymentIdAsync(id, paymentId);

		// Assert
		await result.Should().ThrowAsync<ArgumentNullException>();
	}

	[Fact]
	public async Task UpdatePaymentStatusAsync_GivenValidArguments_ShouldReturnResponseWithUpdatedPaymentStatus()
	{
		// Arrange
		int? id = _fixture.Create<int>();
		string paymentId = _fixture.Create<string>();
		OrderHeader orderHeader = _fixture.Build<OrderHeader>().With(o => o.ApplicationUser, null as ApplicationUser).Create();
		OrderHeaderResponse expected = _mapper.Map<OrderHeaderResponse>(orderHeader);
		expected.PaymentID = paymentId;
		expected.PaymentDate = DateTime.Now;
		expected.PaymentDueDate = null;

		_orderHeaderRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(orderHeader);

		// Act
		var result = await _orderHeaderService.UpdatePaymentIdAsync(id, paymentId);

		// Assert
		result.Should().BeEquivalentTo(expected, x => x.Excluding(o => o.PaymentDate));
		result.PaymentDate.Should().NotBe((default));
	}
	#endregion
}

