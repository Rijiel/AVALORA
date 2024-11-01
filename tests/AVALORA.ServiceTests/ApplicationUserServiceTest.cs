using AutoFixture;
using AutoMapper;
using AVALORA.Core.AutoMapperProfiles;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Enums;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using Moq;

namespace AVALORA.ServiceTests;

public class ApplicationUserServiceTest
{
	private readonly Fixture _fixture;
	private readonly IMapper _mapper;
	private readonly IQueryable<IdentityRole> _roles;

	private readonly Mock<IUnitOfWork> _unitOfWorkMock;
	private readonly Mock<IApplicationUserRepository> _applicationUserRepositoryMock;
	private readonly Mock<IRoleStore<IdentityRole>> _roleStoreMock;
	private readonly Mock<RoleManager<IdentityRole>> _roleManagerMock;

	private readonly IUnitOfWork _unitOfWork;
	private readonly IApplicationUserRepository _applicationUserRepository;
	private readonly IApplicationUserService _applicationUserService;
	private readonly RoleManager<IdentityRole> _roleManager;

	public ApplicationUserServiceTest()
	{
		_fixture = new Fixture();
		_mapper = new MapperConfiguration(cfg => cfg.AddProfile<AutoMapperProfile>()).CreateMapper();
		_roles = new List<IdentityRole>() 
		{ 
			new() { Id = Guid.NewGuid().ToString(), Name = Role.Admin.ToString() },
			new() { Id = Guid.NewGuid().ToString(), Name = Role.User.ToString() }
		}.AsQueryable();

		_unitOfWorkMock = new Mock<IUnitOfWork>();
		_applicationUserRepositoryMock = new Mock<IApplicationUserRepository>();
		_roleStoreMock = new Mock<IRoleStore<IdentityRole>>();
		_roleManagerMock = new Mock<RoleManager<IdentityRole>>(_roleStoreMock.Object, null, null, null, null);

		_unitOfWork = _unitOfWorkMock.Object;
		_applicationUserRepository = _applicationUserRepositoryMock.Object;
		_roleManager = _roleManagerMock.Object;

		_applicationUserService = new ApplicationUserService(_applicationUserRepository, _mapper, _unitOfWork, _roleManager);
	}

	[Theory]
	[InlineData(null)]
	[InlineData("InvalidString")]
	public void GetRoleOptions_GivenInvalidRoleToCompare_ShouldReturnSelectListItemListWithoutSelectedItem(string? roleToCompare)
	{
		// Arrange
		_roleManagerMock.Setup(x => x.Roles).Returns(_roles);

		// Act
		var result = _applicationUserService.GetRoleOptions(roleToCompare);

		// Assert
		result.Should().NotBeEmpty();
		result.Should().BeAssignableTo<List<SelectListItem>>();
		result.Should().NotContain(r => r.Selected);
	}

	[Fact]
	public void GetRoleOptions_GivenValidRoleToCompare_ShouldReturnSelectListItemListWithSelectedItem()
	{
		// Arrange
		string roleToCompare = Role.Admin.ToString();

		_roleManagerMock.Setup(x => x.Roles).Returns(_roles);

		// Act
		var result = _applicationUserService.GetRoleOptions(roleToCompare);

		// Assert
		result.Should().NotBeEmpty();
		result.Should().BeAssignableTo<List<SelectListItem>>();
		result.Should().Contain(r => r.Text == roleToCompare && r.Selected);
	}
}

