using AVALORA.Core.Domain.Models;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Dto.OrderHeaderDtos;

public class OrderHeaderResponse
{
	public int Id { get; set; }
	public string ApplicationUserId { get; set; } = null!;
	public ApplicationUser? ApplicationUser { get; set; }
	public string Name { get; set; } = null!;
	public string PhoneNumber { get; set; } = null!;
	public string Address { get; set; } = null!;
	public string Email { get; set; } = null!;
	public PaymentStatus PaymentStatus { get; set; }
	public OrderStatus OrderStatus { get; set; }
	public DateTime OrderDate { get; set; }
	public DateTime EstimatedFromDate { get; set; }
	public DateTime EstimatedToDate { get; set; }
	public DateTime? ShippingDate { get; set; }
	public DateTime? PaymentDate { get; set; }
	public DateTime? PaymentDueDate { get; set; }
	public string? Carrier { get; set; }
	public string? TrackingNumber { get; set; }
	public string? PaymentID { get; set; }
}

