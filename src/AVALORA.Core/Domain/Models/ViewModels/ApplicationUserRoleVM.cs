using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models.ViewModels;

/// <summary>
/// View model for application user and role options.
/// </summary>
public class ApplicationUserRoleVM
{
	public string Id { get; set; } = null!;

	public string Name { get; set; } = null!;

	[Required]
	public string Role { get; set; } = null!;

	[ValidateNever]
	public IEnumerable<SelectListItem> Roles { get; set; } = null!;
}

