using AVALORA.Core.Domain.Models;
using System.Text.Json.Nodes;

namespace AVALORA.Core.ServiceContracts;

public interface IPaymentService
{
	/// <summary>
	/// Retrieves a PayPal access token asynchronously.
	/// </summary>
	/// <param name="paypal">The PayPal settings.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the request. 
	/// Defaults to the default cancellation token.</param>
	/// <returns>A task containing the PayPal access token.</returns>
	Task<string> GetPaypalAccessTokenAsync(PaypalSettings paypal, CancellationToken cancellationToken = default);

	/// <summary>
	/// Sends a POST request to the specified URL and returns the response asynchronously.
	/// </summary>
	/// <param name="url">The URL to send the request to.</param>
	/// <param name="authHeaderValue">The authentication header value.</param>
	/// <param name="httpContent">The HTTP content to send with the request.</param>
	/// <param name="cancellationToken">A token that can be used to cancel the request. 
	/// Defaults to the default cancellation token.</param>
	/// <returns>A task containing the response from the POST request, or null if no response is received.</returns>
	Task<JsonNode?> SendRequestAsync(string url, string authHeaderValue, HttpContent httpContent,
		CancellationToken cancellationToken = default);
}

