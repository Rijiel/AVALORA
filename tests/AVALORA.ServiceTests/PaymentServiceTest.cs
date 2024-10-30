using AutoFixture;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.Services;
using FluentAssertions;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text.Json.Nodes;

namespace AVALORA.ServiceTests;

public class PaymentServiceTest
{
	private readonly Fixture _fixture;

	private readonly Mock<HttpMessageHandler> _httpMessageHandlerMock;
	private readonly HttpClient _httpClient;

	private readonly IPaymentService _paymentService;

	public PaymentServiceTest()
	{
		_fixture = new Fixture();

		_httpMessageHandlerMock = new Mock<HttpMessageHandler>();
		_httpClient = new HttpClient(_httpMessageHandlerMock.Object);

		_paymentService = new PaymentService(_httpClient);

	}

	#region SendRequestAsync
	[Fact]
	public async Task SendRequestAsync_GivenInvalidArguments_ShouldReturnNull()
	{
		// Arrange
		string url = "http://www.invalidurl.com";
		string authHeaderValue = "Basic QWxhZGRpbjpvcGVuc2VzYW1l";
		var httpContent = new StringContent(string.Empty);
		var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

		_httpMessageHandlerMock.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(httpResponse);

		// Act
		var result = await _paymentService.SendRequestAsync(url, authHeaderValue, httpContent);

		// Assert
		result.Should().BeNull();
	}

	[Fact]
	public async Task SendRequestAsync_GivenValidArguments_ShouldReturnResponse()
	{
		// Arrange
		string url = "http://www.someurl.com";
		string authHeaderValue = "Basic QWxhZGRpbjpvcGVuc2VzYW1l";

		string expected = "{\"response\":\"value\"}";
		var httpContent = new StringContent(expected, encoding: System.Text.Encoding.UTF8);
		var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = httpContent
		};

		_httpMessageHandlerMock.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(httpResponse);

		// Act
		JsonNode? result = await _paymentService.SendRequestAsync(url, authHeaderValue, httpContent);

		// Assert
		result.Should().NotBeNull();
		result.Should().BeAssignableTo<JsonNode>();
		result?.ToJsonString().Should().Be(expected);
	}
	#endregion

	#region GetPaypalAccessTokenAsync
	[Fact]
	public async Task GetPaypalAccessTokenAsync_GivenInvalidPaypalSettings_ShouldThrowException()
	{
		// Arrange
		var paypalSettings = new PaypalSettings();
		var httpResponse = new HttpResponseMessage(HttpStatusCode.BadRequest);

		_httpMessageHandlerMock.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(httpResponse);

		// Act
		Func<Task> result = async () => await _paymentService.GetPaypalAccessTokenAsync(paypalSettings);

		// Assert
		await result.Should().ThrowAsync<Exception>();
	}

	[Fact]
	public async Task GetPaypalAccessTokenAsync_GivenValidPaypalSettings_ShouldReturnAccessToken()
	{
		// Arrange
		var paypalSettings = new PaypalSettings()
		{
			SandboxURL = "https://api.sandbox.paypal.com"
		};
		string content = "{\"access_token\":\"value\"}";
		var httpContent = new StringContent(content, encoding: System.Text.Encoding.UTF8);
		var httpResponse = new HttpResponseMessage(HttpStatusCode.OK)
		{
			Content = httpContent
		};

		_httpMessageHandlerMock.Protected()
			.Setup<Task<HttpResponseMessage>>(
				"SendAsync",
				ItExpr.IsAny<HttpRequestMessage>(),
				ItExpr.IsAny<CancellationToken>())
			.ReturnsAsync(httpResponse);

		// Act
		var result = await _paymentService.GetPaypalAccessTokenAsync(paypalSettings);

		// Assert
		result.Should().NotBeNull();
	}
	#endregion
}

