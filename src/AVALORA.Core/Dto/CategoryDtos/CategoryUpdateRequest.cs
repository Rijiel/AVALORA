using AVALORA.Core.Domain.Models.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.CategoryDtos;

public class CategoryUpdateRequest
{
	[Required]
	public int Id { get; set; }

	[Required]
	[StringLength(25)]
	[RegularExpression(@"^[A-Z][a-zA-Z]+$", ErrorMessage = "Name must start with a capital letter and contain only letters.")]
	[UniqueCategoryName]
    [DisplayName("Category Name")]
	public string Name { get; set; } = null!;
}

