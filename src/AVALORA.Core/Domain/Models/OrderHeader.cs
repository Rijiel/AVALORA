﻿using AVALORA.Core.Domain.Models.Validations;
using AVALORA.Core.Enums;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVALORA.Core.Domain.Models;

public class OrderHeader
{
	public int Id { get; set; }

	[Required]
	public string ApplicationUserId { get; set; } = null!;

	[ForeignKey(nameof(ApplicationUserId))]
	public ApplicationUser? ApplicationUser { get; set; }

	[Required]
	[StringLength(50)]
	[RegularExpression(@"^[a-zA-Z]{4,}(?: [a-zA-Z]+){0,2}$", ErrorMessage = "Name can only contain letters, " +
	"spaces, and periods, and must be at least 4 characters long.")]
	[DisplayName("Full Name")]
	public string Name { get; set; } = null!;

	[Required]
	[Phone]
	[RegularExpression(@"^[+]?\d{7,15}$", ErrorMessage = "Invalid phone number format.")]
	[Column(TypeName = "varchar(15)")]
	[DisplayName("Phone Number")]
	public string PhoneNumber { get; set; } = null!;

	[Required]
	[StringLength(50)]
	public string Address { get; set; } = null!;

	[Required]
	[EmailAddress]
	[StringLength(50)]
	public string Email { get; set; } = null!;

	[Required]
	[DisplayName("Payment Status")]
	public PaymentStatus PaymentStatus { get; set; }

	[Required]
	[DisplayName("Order Status")]
	public OrderStatus OrderStatus { get; set; }

	[Required]
	[DataType(DataType.Date)]
	[CurrentDate(ErrorMessage = "Order date cannot be in the past")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime OrderDate { get; set; }

	[Required]
	[DataType(DataType.Date)]
	[CurrentDate(ErrorMessage = "Estimated date cannot be in the past")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime EstimatedFromDate { get; set; }

	[Required]
	[DataType(DataType.Date)]
	[CurrentDate(ErrorMessage = "Estimated date cannot be in the past")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime EstimatedToDate { get; set; }

	[DataType(DataType.Date)]
	[CurrentDate(ErrorMessage = "Shipping date cannot be in the past")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime? ShippingDate { get; set; }

	[DataType(DataType.Date)]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime? PaymentDate { get; set; }

	[DataType(DataType.Date)]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime? PaymentDueDate { get; set; }

	[StringLength(50)]
	public string? Carrier { get; set; }

	[UniqueTrackingNumber]
	[Column(TypeName = "varchar(15)")]
	public string? TrackingNumber { get; set; }

	[Column(TypeName = "varchar(20)")]
	public string? PaymentID { get; set; }
}

