using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Domain.Models;

public class ApplicationUser : IdentityUser
{
	[Required]
	[StringLength(50)]
	[RegularExpression(@"^[a-zA-Z]{4,}(?: [a-zA-Z]+){0,2}$", ErrorMessage = "Name can only contain letters, " +
	"spaces, and periods, and must be at least 4 characters long.")]
	[DisplayName("Full Name")]
	public string Name { get; set; } = null!;

	[StringLength(50)]
	public string? Address { get; set; }
}

