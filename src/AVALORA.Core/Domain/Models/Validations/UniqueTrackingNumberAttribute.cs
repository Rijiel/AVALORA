using AVALORA.Core.ServiceContracts;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.Validations;

/// <summary>
/// Custom validation attribute to ensure a tracking number is unique.
/// </summary>
public class UniqueTrackingNumberAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var serviceUnitOfWork = validationContext.GetRequiredService<IServiceUnitOfWork>();

		if (serviceUnitOfWork.OrderHeaderSevice.GetAsync(c => c.TrackingNumber == (string?)value).GetAwaiter().GetResult() != null)
			return new ValidationResult("Please enter a unique tracking number.");

		return ValidationResult.Success;
	}
}

