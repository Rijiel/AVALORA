using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.OrderHeaderDtos;

public class OrderHeaderValidatorDTO
{
    public int Id { get; set; }

    [Required]
    public string ApplicationUserId { get; set; } = null!;

    [Required]
    public string Name { get; set; } = null!;

    [Required]
    public string Address { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [Required]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    public string PaymentStatus { get; set; } = null!;

    [Required]
    public string OrderStatus { get; set; } = null!;

    [Required]
    public string Carrier { get; set; } = null!;

    [Required]
    public string TrackingNumber { get; set; } = null!;
}

