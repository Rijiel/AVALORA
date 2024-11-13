using AVALORA.Core.Domain.Models;
using AVALORA.Core.ServiceContracts;
using SerilogTimings;
using System.Text;
using System.Text.Json.Nodes;

namespace AVALORA.Core.Services;

public class PaymentService : IPaymentService
{
	private readonly HttpClient _httpClient;

	public PaymentService(HttpClient httpClient)
	{
		_httpClient = httpClient;
	}

	public async Task<string> GetPaypalAccessTokenAsync(PaypalSettings paypal, CancellationToken cancellationToken = default)
	{
		string accessToken = "";
		string url = paypal.SandboxURL + "/v1/oauth2/token";
		string authHeaderValue = "Basic " + Convert.ToBase64String(
            Encoding.UTF8.GetBytes(paypal.ClientID + ":" + paypal.SecretKey));
		var httpContent = new StringContent("grant_type=client_credentials", null, "application/x-www-form-urlencoded");

		var jsonReponse = await SendRequestAsync(url, authHeaderValue, httpContent, cancellationToken);

		if (jsonReponse != null)
			accessToken = jsonReponse["access_token"]?.ToString() ?? "";

		return accessToken;
	}

	public async Task<JsonNode?> SendRequestAsync(string url, string authHeaderValue, HttpContent httpContent,
		CancellationToken cancellationToken = default)
	{
        using(Operation.Time("Send request to paypal"))
        {
            _httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", authHeaderValue);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = httpContent
            };

            var httpResponse = await _httpClient.SendAsync(requestMessage, cancellationToken);

            if (httpResponse.IsSuccessStatusCode)
            {
                var response = await httpResponse.Content.ReadAsStringAsync();
                return JsonNode.Parse(response);
            }
        }		

		return null;
	}
}

