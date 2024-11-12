using AVALORA.Core.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AVALORA.IntegrationTests;

internal class MockAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
	public MockAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
		: base(options, logger, encoder)
	{
	}

	protected override Task<AuthenticateResult> HandleAuthenticateAsync()
	{
		var claims = new[]
		{
			new Claim(ClaimTypes.Name, "Test User"),
			new Claim(ClaimTypes.Role, Role.Admin.ToString())
		};
		var identity = new ClaimsIdentity(claims, IdentityConstants.ApplicationScheme);
		var principal = new ClaimsPrincipal(identity);
		var ticket = new AuthenticationTicket(principal, IdentityConstants.ApplicationScheme);

		var result = AuthenticateResult.Success(ticket);
		return Task.FromResult(result);
	}
}

