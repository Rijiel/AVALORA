using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for order process, containing order header and summary details.
/// </summary>
public class OrderVM
{
    [Required]
    public OrderHeaderResponse OrderHeaderResponse { get; set; } = null!;

    public OrderSummaryResponse? OrderSummaryResponse { get; set; }
}

