using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace AVALORA.Core.Dto.ApplicationUserDtos;

public class ApplicationUserUpdateRequest
{
	public string Id { get; set; } = null!;

    [Required]
    [StringLength(50)]
	[RegularExpression(@"^[a-zA-Z]{4,}(?: [a-zA-Z]+){0,2}$", ErrorMessage = "Name can only contain letters, " +
	"spaces, and periods, and must be at least 4 characters long.")]
	[DisplayName("Full Name")]
	public string Name { get; set; } = null!;

    [Required]
	[EmailAddress]
    [StringLength(50)]
	public string Email { get; set; } = null!;

	[StringLength(50)]
    public string? Address { get; set; }

    [Required]
    [Phone]
    [StringLength(15)]
    [DisplayName("Phone Number")]
    public string PhoneNumber { get; set; } = null!;
}

