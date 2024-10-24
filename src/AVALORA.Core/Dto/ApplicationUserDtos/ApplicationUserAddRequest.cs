using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.ApplicationUserDtos;

public class ApplicationUserAddRequest
{
    [Required]
    [StringLength(50)]
    [RegularExpression("^[a-zA-Z]+$", ErrorMessage = "Name can only contain letters")]
    [DisplayName("Full Name")]
    public string Name { get; set; } = null!;

    [Required]
    public string Email { get; set; } = null!;

    [StringLength(50)]
    public string? Address { get; set; }

    [Required]
    [Phone]
    [StringLength(15)]
    [Display(Name = "Phone Number")]
    public string PhoneNumber { get; set; } = null!;
}

