using AVALORA.Core.ServiceContracts;
using Microsoft.Extensions.DependencyInjection;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.Validations;

/// <summary>
/// Custom validation attribute to ensure a category name is unique.
/// </summary>
public class UniqueCategoryNameAttribute : ValidationAttribute
{
	protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
	{
		var serviceUnitOfWork = validationContext.GetRequiredService<IServiceUnitOfWork>();

		if (serviceUnitOfWork.CategoryService.GetAsync(c => c.Name == (string?)value).GetAwaiter().GetResult() != null)
			return new ValidationResult("Category already exists.");

		return ValidationResult.Success;
	}
}

