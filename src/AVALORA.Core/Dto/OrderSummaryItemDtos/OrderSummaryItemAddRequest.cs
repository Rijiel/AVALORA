using AVALORA.Core.Enums;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.OrderSummaryItemDtos;

public class OrderSummaryItemAddRequest
{
	[Required]
	public int ProductId { get; set; }

	[Required]
	[Range(1, 20, ErrorMessage = "You can only order maximum 20 items at a time")]
	public int Count { get; set; }

	[Required]
	public int OrderSummaryId { get; set; }

    [Required(ErrorMessage = "Please specify your preferred color")]
	public Color Color { get; set; }
}

