using AVALORA.Core.Dto;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace AVALORA.Core.Helpers;

/// <summary>
/// Provides helper methods for validation and model state management.
/// </summary>
public static class ValidationHelper
{
	/// <summary>
	/// Adds model state errors from TempData to the controller's model state.
	/// </summary>
	/// <param name="controller">The controller instance.</param>
	/// <param name="parentPropertyName">The parent property name (optional).</param>
	public static void AddModelState(Controller controller, string? parentPropertyName = null)
	{
		// Check if TempData contains model state errors
		if (controller.TempData.TryGetValue("ModelState", out object? value))
		{
			var modelStateString = value as string;
			if (!string.IsNullOrEmpty(modelStateString))
			{
				var modelStateDtos = JsonSerializer.Deserialize<List<ModelStateDTO>>(modelStateString)!;

				// Add each error to the model state
				foreach (var modelStateDto in modelStateDtos)
				{
					if (modelStateDto.Key != null && modelStateDto.ErrorMessage != null)
						controller.ModelState.AddModelError(
							!string.IsNullOrEmpty(parentPropertyName)
							? parentPropertyName + "." + modelStateDto.Key
							: modelStateDto.Key, modelStateDto.ErrorMessage);
				}
			}
		}
	}

	/// <summary>
	/// Validates the specified model and updates the controller's model state accordingly.
	/// </summary>
	/// <param name="model">The model to validate.</param>
	/// <param name="controller">The controller instance.</param>
	/// <returns>True if the model is valid; otherwise, false.</returns>
	public static bool IsModelStateValid(object? model, Controller controller)
	{
		if (model == null)
			return false;

		var validationContext = new ValidationContext(model);
		var validationResults = new List<ValidationResult>();

		bool isValid = Validator.TryValidateObject(model, validationContext, validationResults, true);

		// If the model is invalid, add errors to the model state
		if (!isValid)
		{
			List<ModelStateDTO> modelStateDTOs = [];
			foreach (var error in validationResults)
			{
				if (error.ErrorMessage != null)
				{
					if (error.MemberNames?.FirstOrDefault() is string property)
					{
						controller.ModelState.AddModelError(property, error.ErrorMessage);

						// Serialize to DTO for Redirection
						var modelStateDTO = new ModelStateDTO()
						{
							Key = property,
							ErrorMessage = error.ErrorMessage
						};
						modelStateDTOs.Add(modelStateDTO);
					}
				}
			}

			// Use a serialized version of the model state for TempData
			controller.TempData["ModelState"] = JsonSerializer.Serialize(modelStateDTOs);
		}

		return isValid;
	}
}

