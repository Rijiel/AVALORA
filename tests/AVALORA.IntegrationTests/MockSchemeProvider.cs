using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace AVALORA.IntegrationTests;

internal class MockSchemeProvider : AuthenticationSchemeProvider
{
    public MockSchemeProvider(IOptions<AuthenticationOptions> options) : base(options)
    {        
    }

    protected MockSchemeProvider(IOptions<AuthenticationOptions> options, 
        IDictionary<string, AuthenticationScheme> schemes) : base(options, schemes)
	{        
	}

    public override Task<AuthenticationScheme?> GetSchemeAsync(string name)
    {
		AuthenticationScheme mockScheme = new(IdentityConstants.ApplicationScheme, IdentityConstants.ApplicationScheme, typeof(MockAuthHandler));

		return Task.FromResult(mockScheme)!;
	}
}

