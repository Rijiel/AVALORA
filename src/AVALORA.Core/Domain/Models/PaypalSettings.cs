namespace AVALORA.Core.Domain.Models;

public class PaypalSettings
{
	public string ClientID { get; set; } = "";
	public string SecretKey { get; set; } = "";
	public string SandboxURL { get; set; } = "";
}

