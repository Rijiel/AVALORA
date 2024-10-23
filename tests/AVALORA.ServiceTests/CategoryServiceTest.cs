using AutoFixture;
using AutoMapper;
using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Moq;
using System.Linq.Expressions;

namespace AVALORA.ServiceTests;

public class CategoryServiceTest
{
    private readonly Fixture _fixture;
    private readonly IMapper _mapper;

    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;

    private readonly IUnitOfWork _unitOfWork;
    private readonly ICategoryRepository _categoryRepository;
    private readonly ICategoryService _categoryService;

    public CategoryServiceTest()
    {
        _fixture = new Fixture();
        _mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();

        _unitOfWorkMock = new Mock<IUnitOfWork>();
        _categoryRepositoryMock = new Mock<ICategoryRepository>();

        _unitOfWork = _unitOfWorkMock.Object;
        _categoryRepository = _categoryRepositoryMock.Object;

        _categoryService = new CategoryService(_categoryRepository, _mapper, _unitOfWork);
    }

    #region GetAllAsync
    [Fact]
    public async Task GetAllAsync_ShouldReturnEmptyByDefault()
    {
        // Arrange
        var categories = new List<Category>();
        _categoryRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string[]>())).ReturnsAsync(categories);

        // Act
        List<CategoryResponse> result = await _categoryService.GetAllAsync();

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAllAsync_GivenCategories_ShouldReturnCategoryResponses()
    {
        // Arrange
        List<Category> categories = _fixture.CreateMany<Category>(10).ToList();
        var expected = _mapper.Map<List<CategoryResponse>>(categories);

        _categoryRepositoryMock.Setup(x => x.GetAllAsync(It.IsAny<Expression<Func<Category, bool>>>(), It.IsAny<string[]>())).ReturnsAsync(categories);

        // Act
        List<CategoryResponse> result = await _categoryService.GetAllAsync();

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAllAsync_GivenFilter_ShouldReturnFilteredCategoryResponses()
    {
        // Arrange
        List<Category> categories = _fixture.CreateMany<Category>(10).ToList();
        Expression<Func<Category, bool>> filter = x => x.Name.Contains("Test");
        categories = categories.Where(filter.Compile()).ToList();

        var expected = _mapper.Map<List<CategoryResponse>>(categories);

        _categoryRepositoryMock.Setup(x => x.GetAllAsync(filter, It.IsAny<string[]>())).ReturnsAsync(categories);


        // Act
        List<CategoryResponse> result = await _categoryService.GetAllAsync(filter);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region GetAsync
    [Fact]
    public async Task GetAsync_GivenCategoryAndFilter_ShouldReturnFilteredCategoryResponse()
    {
        // Arrange
        var category = _fixture.Create<Category>();
        Expression<Func<Category, bool>> filter = x => x.Name.Contains("Test");
        var expected = _mapper.Map<CategoryResponse>(category);

        _categoryRepositoryMock.Setup(x => x.GetAsync(filter, It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);


        // Act
        CategoryResponse? result = await _categoryService.GetAsync(filter);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task GetAsync_GivenFilterWithNoResult_ShouldReturnNull()
    {
        // Arrange
        Category? category = null;
        Expression<Func<Category, bool>> filter = x => x.Name.Contains("Test");

        _categoryRepositoryMock.Setup(x => x.GetAsync(filter, It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        CategoryResponse? result = await _categoryService.GetAsync(filter);

        // Assert
        result.Should().BeNull();
    }
    #endregion

    #region GetByIdAsync
    [Fact]
    public async Task GetByIdAsync_GivenNullId_ShouldReturnNull()
    {
        // Arrange
        Category? category = null;
        int? id = null;

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        CategoryResponse? result = await _categoryService.GetByIdAsync(id); ;

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_GivenInvalidId_ShouldReturnNull()
    {
        // Arrange
        Category? category = null;
        int? id = _fixture.Create<int>();

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        CategoryResponse? result = await _categoryService.GetByIdAsync(id);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetByIdAsync_GivenValidId_ShouldReturnCategoryResponse()
    {
        // Arrange
        Category? category = _fixture.Create<Category>();
        int? id = _fixture.Create<int>();
        var expected = _mapper.Map<CategoryResponse>(category);

        _categoryRepositoryMock.Setup(x => x.GetByIdAsync(id, It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);


        // Act
        CategoryResponse? result = await _categoryService.GetByIdAsync(id);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region AddAsync
    [Fact]
    public async Task AddAsync_GivenNullCategoryAddRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        CategoryAddRequest? categoryAddRequest = null;

        _categoryRepositoryMock.Setup(x => x.Add(It.IsAny<Category>()));

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.AddAsync(categoryAddRequest);
        };

        // Assert
        await result.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task AddAsync_GivenCategoryAddRequest_ShouldReturnCategoryResponse()
    {
        // Arrange
        var categoryAddRequest = _fixture.Create<CategoryAddRequest>();
        var category = _mapper.Map<Category>(categoryAddRequest);
        var expected = _mapper.Map<CategoryResponse>(category);

        _categoryRepositoryMock.Setup(x => x.Add(It.IsAny<Category>()));

        // Act
        var result = await _categoryService.AddAsync(categoryAddRequest);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }
    #endregion

    #region AddRangeAsync
    [Fact]
    public async Task AddRangeAsync_GivenEmptyList_ShouldReturnEmptyList()
    {
        // Arrange
        List<CategoryAddRequest>? categoryAddRequests = [];

        _categoryRepositoryMock.Setup(x => x.AddRange(It.IsAny<List<Category>>()));

        // Act
        List<CategoryResponse> result = await _categoryService.AddRangeAsync(categoryAddRequests);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task AddRangeAsync_GivenCategoryAddRequests_ShouldReturnCategoryResponses()
    {
        // Arrange
        var categoryAddRequests = _fixture.CreateMany<CategoryAddRequest>(10).ToList();
        var categories = _mapper.Map<List<Category>>(categoryAddRequests);
        var expected = _mapper.Map<List<CategoryResponse>>(categories);

        _categoryRepositoryMock.Setup(x => x.AddRange(It.IsAny<List<Category>>()));

        // Act
        List<CategoryResponse> result = await _categoryService.AddRangeAsync(categoryAddRequests);

        // Assert
        result.Should().BeEquivalentTo(expected);
        _categoryRepositoryMock.Verify(x => x.AddRange(It.IsAny<List<Category>>()), Times.Once);
    }
    #endregion

    #region UpdateAsync
    [Fact]
    public async Task UpdateAsync_GivenNullCategoryUpdateRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        CategoryUpdateRequest? categoryUpdateRequest = null;

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.UpdateAsync(categoryUpdateRequest);
        };

        // Assert
        await result.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdateAsync_GivenValidCategoryUpdateRequest_ShouldReturnCategoryResponse()
    {
        // Arrange
        CategoryUpdateRequest? categoryUpdateRequest = _fixture.Create<CategoryUpdateRequest>();
        Category category = _mapper.Map<Category>(categoryUpdateRequest);
        var expected = _mapper.Map<CategoryResponse>(category);

        _categoryRepositoryMock.Setup(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        CategoryResponse result = await _categoryService.UpdateAsync(categoryUpdateRequest);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdateAsync_GivenValidCategoryNameUpdateRequest_ShouldReturnCategoryResponseWithUpdatedName()
    {
        // Arrange
        var expected = _fixture.Build<Category>().With(x => x.Id, 1).Create();
        var categoryUpdateRequest = _fixture.Build<CategoryUpdateRequest>().With(x => x.Id, 1).Create();

        _categoryRepositoryMock.Setup(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(expected);

        // Act
        CategoryResponse result = await _categoryService.UpdateAsync(categoryUpdateRequest);

        // Assert
        expected.Name.Should().Be(categoryUpdateRequest.Name);
    }
    #endregion

    #region UpdatePartialAsync
    [Fact]
    public async Task UpdatePartialAsync_GivenNullCategoryUpdateRequest_ShouldThrowArgumentNullException()
    {
        // Arrange
        CategoryUpdateRequest? categoryUpdateRequest = null;
        var propertyNames = _fixture.CreateMany<string>(10).ToArray();

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.UpdatePartialAsync(categoryUpdateRequest, propertyNames);
        };

        // Assert
        await result.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task UpdatePartialAsync_GivenEmptyPropertyNameList_ShouldThrowArgumentException()
    {
        // Arrange
        CategoryUpdateRequest? categoryUpdateRequest = null;
        string[] propertyNames = [];

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.UpdateAsync(categoryUpdateRequest);
        };

        // Assert
        await result.Should().ThrowAsync<ArgumentException>();
    }

    [Fact]
    public async Task UpdatePartialAsync_GivenValidCategoryUpdateRequestAndPropertyNames_ShouldReturnCategoryResponse()
    {
        // Arrange
        CategoryUpdateRequest? categoryUpdateRequest = _fixture.Create<CategoryUpdateRequest>();
        var propertyNames = _fixture.CreateMany<string>(10).ToArray();
        Category category = _mapper.Map<Category>(categoryUpdateRequest);
        var expected = _mapper.Map<CategoryResponse>(category);

        _categoryRepositoryMock.Setup(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        CategoryResponse result = await _categoryService.UpdatePartialAsync(categoryUpdateRequest, propertyNames);

        // Assert
        result.Should().BeEquivalentTo(expected);
    }

    [Fact]
    public async Task UpdatePartialAsync_GivenValidCategoryUpdateRequestWithNamePropertyParams_ShouldReturnCategoryResponseWithUpdatedName()
    {
        // Arrange
        var expected = _fixture.Build<Category>().With(x => x.Id, 1).Create();
        var categoryUpdateRequest = _fixture.Build<CategoryUpdateRequest>().With(x => x.Id, 1).Create();
        var propertyNames = new[] { nameof(Category.Name) };

        _categoryRepositoryMock.Setup(
            x => x.GetByIdAsync(It.IsAny<int>(), It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(expected);

        // Act
        CategoryResponse result = await _categoryService.UpdatePartialAsync(categoryUpdateRequest, propertyNames);

        // Assert
        expected.Name.Should().Be(categoryUpdateRequest.Name);
    }
    #endregion

    #region RemoveAsync
    [Fact]
    public async Task RemoveAsync_GivenNullId_ShouldThrowArgumentNullException()
    {
        // Arrange
        int? id = null;

        _categoryRepositoryMock.Setup(x => x.Remove(It.IsAny<Category>()));

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.RemoveAsync(id);
        };

        // Assert
        await result.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RemoveAsync_GivenInvalidId_ShouldThrowKeyNotFoundException()
    {
        // Arrange
        int? id = _fixture.Create<int>();
        Category? category = null;

        _categoryRepositoryMock.Setup(x => x.Remove(It.IsAny<Category>()));
        _categoryRepositoryMock.Setup(
            x => x.GetByIdAsync(id, It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.RemoveAsync(id);
        };

        // Assert
        await result.Should().ThrowAsync<KeyNotFoundException>();
    }

    [Fact]
    public async Task RemoveAsync_GivenValidId_ShouldCallRemoveOnce()
    {
        // Arrange
        int? id = _fixture.Create<int>();
        Category category = _fixture.Create<Category>();

        _categoryRepositoryMock.Setup(x => x.Remove(It.IsAny<Category>()));
        _categoryRepositoryMock.Setup(
            x => x.GetByIdAsync(id, It.IsAny<bool>(), It.IsAny<string[]>())).ReturnsAsync(category);

        // Act
        await _categoryService.RemoveAsync(id);

        // Assert
        _categoryRepositoryMock.Verify(x => x.Remove(It.IsAny<Category>()), Times.Once);
    }
    #endregion

    #region RemoveRangeAsync
    [Fact]
    public async Task RemoveRangeAsync_GivenNullCategoryResponses_ShouldThrowArgumentNullException()
    {
        // Arrange
        List<CategoryResponse>? categoryResponses = null;

        _categoryRepositoryMock.Setup(x => x.RemoveRange(It.IsAny<List<Category>>()));

        // Act
        Func<Task> result = async () =>
        {
            await _categoryService.RemoveRangeAsync(categoryResponses);
        };

        // Assert
        await result.Should().ThrowAsync<ArgumentNullException>();
    }

    [Fact]
    public async Task RemoveRangeAsync_GivenValidCategoryResponses_ShouldCallRemoveRangeOnce()
    {
        // Arrange
        var categoryResponses = _fixture.CreateMany<CategoryResponse>(10).ToList();

        _categoryRepositoryMock.Setup(x => x.RemoveRange(It.IsAny<List<Category>>()));

        // Act
        await _categoryService.RemoveRangeAsync(categoryResponses);

        // Assert
        _categoryRepositoryMock.Verify(x => x.RemoveRange(It.IsAny<List<Category>>()), Times.Once);
    }
    #endregion
}