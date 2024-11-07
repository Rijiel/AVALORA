using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Enums;
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

	public bool IsApproved => OrderHeaderResponse.OrderStatus == OrderStatus.Approved;
	public bool IsProcessing => OrderHeaderResponse.OrderStatus == OrderStatus.Processing;
	public bool IsShipped => OrderHeaderResponse.OrderStatus == OrderStatus.Shipped;
	public bool IsPending => OrderHeaderResponse.OrderStatus == OrderStatus.Pending;
	public bool IsCancelled => OrderHeaderResponse.OrderStatus == OrderStatus.Cancelled;
	public bool IsPaymentPending => OrderHeaderResponse.PaymentStatus == PaymentStatus.Pending;
	public bool IsPaymentApproved => OrderHeaderResponse.PaymentStatus == PaymentStatus.Approved;
	public bool IsPaymentDelayed => OrderHeaderResponse.PaymentStatus == PaymentStatus.DelayedPayment;
	public bool IsPaymentCancelled => OrderHeaderResponse.PaymentStatus == PaymentStatus.Cancelled;
}

