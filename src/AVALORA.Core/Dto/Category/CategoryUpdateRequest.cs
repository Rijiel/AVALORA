using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.Category;

public class CategoryUpdateRequest
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; } = null!;
}

