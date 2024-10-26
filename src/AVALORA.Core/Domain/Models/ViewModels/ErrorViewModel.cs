namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for error pages, containing request ID information.
/// </summary>
public class ErrorViewModel
{
    public string? RequestId { get; set; }

    public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
}
