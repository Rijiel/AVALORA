using AVALORA.Core.Domain.Models.Validations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using AVALORA.Core.Dto.CartItemDtos;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for checkout process, containing only needed properties from order header
/// and summary details.
/// </summary>
public class CheckoutVM
{
	// Needed fields from order header add request
	[Required]
	public string ApplicationUserId { get; set; } = null!;

	[Required]
	[StringLength(50)]
	[RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
	[DisplayName("Full Name")]
	public string Name { get; set; } = null!;

	[Required]
	[Phone]
	[RegularExpression(@"^[+]?\d{7,15}$", ErrorMessage = "Invalid phone number format.")]
	[StringLength(15)]
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
	[DataType(DataType.Date)]
	[CurrentDate(ErrorMessage = "Estimated date cannot be in the past")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime EstimatedFromDate { get; set; }

	[Required]
	[DataType(DataType.Date)]
	[CurrentDate(ErrorMessage = "Estimated date cannot be in the past")]
	[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
	public DateTime EstimatedToDate { get; set; }

	[ValidateNever]
	public IEnumerable<CartItemResponse>? CartItemResponses { get; set; }

	[Required]
	[DataType(DataType.Currency)]
	public double TotalPrice { get; set; }
}

