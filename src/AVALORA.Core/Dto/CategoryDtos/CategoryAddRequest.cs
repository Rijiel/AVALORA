using AVALORA.Core.Domain.Models.Validations;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.CategoryDtos;

public class CategoryAddRequest
{
	[Required]
	[StringLength(50)]
	[RegularExpression(@"^[A-Z][a-zA-Z]+$", ErrorMessage = "Name must start with a capital letter and contain only letters.")]
	[UniqueCategoryName]
	public string Name { get; set; } = null!;
}

