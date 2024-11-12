using AutoFixture;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Enums;
using AVALORA.Infrastructure.DatabaseContext;
using Fizzler.Systems.HtmlAgilityPack;
using FluentAssertions;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace AVALORA.IntegrationTests;

public class CategoriesControllerTest
{
	private readonly MockWebApplicationFactory _factory;
	private readonly Fixture _fixture;

	public CategoriesControllerTest()
	{
		_factory = new MockWebApplicationFactory();
		_fixture = new Fixture();
	}

	[Fact]
	public async Task Index_ShouldReturnViewDisplayingAllCategories()
	{
		// Arrange
		var client = _factory.CreateClient();

		// Act
		var result = await client.GetAsync("/Categories/Index");
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var categoriesDiv = document.QuerySelector("#all-categories");
		var breadcrumb = document.QuerySelector(".breadcrumb");

		// Assert
		result.EnsureSuccessStatusCode();
		categoriesDiv.Should().NotBeNull();
		breadcrumb.Should().NotBeNull();
	}

	#region CreatePost
	[Fact]
	public async Task CreatePost_GivenInvalidModel_ShouldReturnViewWithModelStateErrors()
	{
		// Arrange
		var client = _factory.CreateClient();
		var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Categories/Index");
		var model = new Dictionary<string, string>
		{
			{ "CategoryAddRequest.Name", string.Empty }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var errorSpan = document.SelectSingleNode("//span[@data-valmsg-for=\"CategoryAddRequest.Name\"]");

		// Assert
		result.EnsureSuccessStatusCode();
		errorSpan.Should().NotBeNull();
		errorSpan.InnerText.Should().Contain("The Category Name field is required");
	}

	[Fact]
	public async Task CreatePost_GivenExistingModel_ShouldReturnViewWithModelStateErrors()
	{
		// Arrange
		var client = _factory.CreateClient();
		var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Categories/Index");
		string categoryName = "TestCategoryExisting";
		var model = new Dictionary<string, string>
		{
			{ "CategoryAddRequest.Name", categoryName }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = categoryName };
			context.Categories.Add(category);
			await context.SaveChangesAsync();
		}

		// Act
		var result = await client.SendAsync(postRequest);
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var errorSpan = document.SelectSingleNode("//span[@data-valmsg-for=\"CategoryAddRequest.Name\"]");

		// Assert
		result.EnsureSuccessStatusCode();
		errorSpan.Should().NotBeNull();
		errorSpan.InnerText.Should().Contain("Category already exists.");
	}

	[Fact]
	public async Task CreatePost_GivenValidModel_ShouldCreateCategory()
	{
		// Arrange
		var client = _factory.CreateClient();
		var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Categories/Index");
		string categoryName = "IntegrationTestCategoryA";
		var model = new Dictionary<string, string>
		{
			{ "CategoryAddRequest.Name", categoryName }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var errorSpan = document.SelectSingleNode("//span[@data-valmsg-for=\"CategoryAddRequest.Name\"]");

		// Assert
		result.EnsureSuccessStatusCode();
		errorSpan.Should().NotBeNull();
		errorSpan.InnerText.Should().BeEmpty();
		content.Should().Contain(categoryName);
	}

	[Fact]
	public async Task CreatePost_GivenValidModel_ShouldRedirectToIndex()
	{
		// Arrange
		var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		var postRequest = new HttpRequestMessage(HttpMethod.Post, "/Categories/Index");
		string categoryName = "IntegrationTestCategoryB";
		var model = new Dictionary<string, string>
		{
			{ "CategoryAddRequest.Name", categoryName }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.Redirect);
		result.Headers.Location.Should().Be(new Uri("/Categories/Index", UriKind.Relative));
	}
	#endregion

	#region EditGet
	[Fact]
	public async Task EditGet_GivenInvalidId_ShouldReturnNotFound()
	{
		// Arrange
		var client = _factory.CreateClient();
		int id = 0;

		// Act
		var result = await client.GetAsync($"/Categories/Edit/{id}");
		var content = await result.Content.ReadAsStringAsync();

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.NotFound);
	}

	[Fact]
	public async Task EditGet_GivenValidId_ShouldReturnView()
	{
		// Arrange
		var client = _factory.CreateClient();

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = "IntegrationTestCategoryC" };
			context.Categories.Add(category);
			await context.SaveChangesAsync();
		}

		// Act
		var result = await client.GetAsync("/Categories/Edit/1");
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var breadcrumb = document.QuerySelector(".breadcrumb");
		var hiddenIdField = document.SelectSingleNode("//input[@id='Id']");

		// Assert
		result.EnsureSuccessStatusCode();
		breadcrumb.Should().NotBeNull();
		hiddenIdField.Should().NotBeNull();
	}
	#endregion

	#region EditPost
	[Fact]
	public async Task EditPost_GivenInvalidModel_ShouldReturnViewWithModelStateErrors()
	{
		// Arrange
		var client = _factory.CreateClient();
		int id = 0;

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = "IntegrationTestCategoryC" };
			var entityResponse = context.Categories.Add(category);
			id = entityResponse.Entity.Id;
			await context.SaveChangesAsync();
		}

		var postRequest = new HttpRequestMessage(HttpMethod.Post, $"/Categories/Edit/{id}");
		var model = new Dictionary<string, string>
		{
			{ "Name", string.Empty }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var errorSpan = document.SelectSingleNode("//span[@data-valmsg-for=\"Name\"]");

		// Assert
		result.EnsureSuccessStatusCode();
		errorSpan.Should().NotBeNull();
		errorSpan.InnerText.Should().Contain("The Category Name field is required");
	}

	[Fact]
	public async Task EditPost_GivenExistingModel_ShouldReturnViewWithModelStateErrors()
	{
		// Arrange
		var client = _factory.CreateClient();
		string categoryName = "TestCategoryExistingB";
		int id = 0;
				
		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = categoryName };
			var entityResponse = context.Categories.Add(category);
			id = entityResponse.Entity.Id;
			await context.SaveChangesAsync();
		}

		var postRequest = new HttpRequestMessage(HttpMethod.Post, $"/Categories/Edit/{id}");
		var model = new Dictionary<string, string>
		{
			{ "Name", categoryName }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);
		var content = await result.Content.ReadAsStringAsync();

		var html = new HtmlDocument();
		html.LoadHtml(content);
		var document = html.DocumentNode;

		var errorSpan = document.SelectSingleNode("//span[@data-valmsg-for=\"Name\"]");

		// Assert
		result.EnsureSuccessStatusCode();
		errorSpan.Should().NotBeNull();
		errorSpan.InnerText.Should().Contain("Category already exists.");
	}

	[Fact]
	public async Task EditPost_GivenValidModel_ShouldUpdateCategory()
	{
		// Arrange
		var client = _factory.CreateClient();
		string originalName = "IntegrationTestCategoryD";
		string updatedName = "IntegrationTestCategoryE";
		int id = 0;

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = originalName };
			var entityResponse = context.Categories.Add(category);
			id = entityResponse.Entity.Id;
			await context.SaveChangesAsync();
		}

		var postRequest = new HttpRequestMessage(HttpMethod.Post, $"/Categories/Edit/{id}");
		var model = new Dictionary<string, string>
		{
			{ "Name", updatedName }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);
		var content = await result.Content.ReadAsStringAsync();

		// Assert
		result.EnsureSuccessStatusCode();
		content.Should().NotContain(originalName);
		content.Should().Contain(updatedName);
	}

	[Fact]
	public async Task EditPost_GivenValidModel_ShouldRedirectToIndex()
	{
		// Arrange
		var client = _factory.CreateClient(new WebApplicationFactoryClientOptions { AllowAutoRedirect = false });
		string originalName = "IntegrationTestCategoryF";
		string updatedName = "IntegrationTestCategoryG";
		int id = 0;

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = originalName };
			var entityResponse = context.Categories.Add(category);
			id = entityResponse.Entity.Id;
			await context.SaveChangesAsync();
		}

		var postRequest = new HttpRequestMessage(HttpMethod.Post, $"/Categories/Edit/{id}");
		var model = new Dictionary<string, string>
		{
			{ "Name", updatedName }
		};
		postRequest.Content = new FormUrlEncodedContent(model);

		// Act
		var result = await client.SendAsync(postRequest);

		// Assert
		result.StatusCode.Should().Be(HttpStatusCode.Redirect);
		result.Headers.Location.Should().Be(new Uri("/Categories/Index", UriKind.Relative));
	}
	#endregion

	#region Delete
	[Fact]
	public async Task DeletePost_GivenInvalidId_ShouldReturnJsonSuccessFalse()
	{
		// Arrange
		var client = _factory.CreateClient();
		int id = 123456;

		var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/Categories/Delete/{id}");

		// Act
		var result = await client.SendAsync(deleteRequest);
		var content = await result.Content.ReadAsStringAsync();

		// Assert
		result.EnsureSuccessStatusCode();
		content.Should().Contain("\"success\":false");
	}

	[Fact]
	public async Task DeletePost_GivenCategoryWithAssociatedProduct_ShouldReturnJsonSuccessFalse()
	{
		// Arrange
		var client = _factory.CreateClient();
		int id = 0;

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = "IntegrationTestCategoryH" };
			var entityResponse = context.Categories.Add(category);
			id = entityResponse.Entity.Id;
			await context.SaveChangesAsync();

			var product = new Product
			{
				Name = "IntegrationTestProduct",
				Description = "IntegrationTestProductDescription",
				Price = 1,
				CategoryId = id,
				Colors = [Color.Red]
			};
			context.Products.Add(product);
			await context.SaveChangesAsync();
		}
		var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/Categories/Delete/{id}");

		// Act
		var result = await client.SendAsync(deleteRequest);
		var content = await result.Content.ReadAsStringAsync();

		// Assert
		result.EnsureSuccessStatusCode();
		content.Should().Contain("\"success\":false");
	}

	[Fact]
	public async Task DeletePost_GivenValidId_ShouldReturnJsonSuccessTrue()
	{
		// Arrange
		var client = _factory.CreateClient();
		int id = 0;

		using (var scope = _factory.Services.CreateScope())
		{
			var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
			var category = new Category { Name = "IntegrationTestCategoryI" };
			var entityResponse = context.Categories.Add(category);
			id = entityResponse.Entity.Id;
			await context.SaveChangesAsync();
		}
		var deleteRequest = new HttpRequestMessage(HttpMethod.Delete, $"/Categories/Delete/{id}");

		// Act
		var result = await client.SendAsync(deleteRequest);
		var content = await result.Content.ReadAsStringAsync();

		// Assert
		result.EnsureSuccessStatusCode();
		content.Should().Contain("\"success\":true");
		content.Should().Contain("\"redirectUrl\":\"/Categories/Index\"");
	}
	#endregion
}