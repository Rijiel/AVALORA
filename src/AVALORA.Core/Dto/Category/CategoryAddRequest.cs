using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.Category;

public class CategoryAddRequest
{
    [Required]
    public string Name { get; set; } = null!;
}

