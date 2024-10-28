using AutoFixture;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;

namespace AVALORA.ServiceTests;

public class PagerServiceTest
{
	private readonly Fixture _fixture;
	private readonly IPagerService _pagerService;

	public PagerServiceTest()
	{
		_fixture = new Fixture();
		_pagerService = new PagerService();
	}

	[Fact]
	public void GetPagedItems_GivenPageIsOne_ShouldReturnCorrectItems()
	{
		// Arrange
		var items = new List<string> { "Item1", "Item2", "Item3", "Item4", "Item5" };
		var expected = new List<string> { "Item1", "Item2", "Item3", "Item4", "Item5" };
		var page = 1;
		var pageSize = 5;

		// Act
		var result = _pagerService.GetPagedItems(items, page, pageSize);

		// Assert
		result.Count.Should().Be(expected.Count);
		result.Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void GetPagedItems_GivenPageIsTwo_ShouldReturnCorrectItems()
	{
		// Arrange
		var items = new List<string> { "Item1", "Item2", "Item3", "Item4", "Item5" };
		var expected = new List<string> { "Item4", "Item5" };
		var pagerService = new PagerService();
		var page = 2;
		var pageSize = 3;

		// Act
		var result = _pagerService.GetPagedItems(items, page, pageSize);

		// Assert
		result.Count.Should().Be(expected.Count);
		result.Should().BeEquivalentTo(expected);
	}

	[Fact]
	public void GetPagedItems_GivenPageIsGreaterThanTotalPages_ShouldReturnCorrectItems()
	{
		// Arrange
		var items = new List<string> { "Item1", "Item2", "Item3", "Item4", "Item5" };
		var expected = new List<string>();
		var page = 3;
		var pageSize = 3;

		// Act
		var result = _pagerService.GetPagedItems(items, page, pageSize);

		// Assert
		result.Count.Should().Be(expected.Count);
		result.Should().BeEquivalentTo(expected);
	}
}

