using AVALORA.Core.Domain.Models.Validations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.CategoryDtos;

public class CategoryAddRequest
{
	[Required]
	[StringLength(25)]
	[UniqueCategoryName]
    [DisplayName("Category Name")]
	public string Name { get; set; } = null!;
}

