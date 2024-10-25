using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVALORA.Core.Domain.Models;

public class OrderSummaryItem
{
	public int Id { get; set; }

	[Required]
	public int ProductId { get; set; }

	[ForeignKey(nameof(ProductId))]
	public Product? Product { get; set; }

	[Required]
	[Range(1, 20, ErrorMessage = "You can only order maximum 20 items at a time")]
	public int Count { get; set; }

	[Required]
	public int OrderSummaryId { get; set; }

	[ForeignKey(nameof(OrderSummaryId))]
	public OrderSummary? OrderSummary { get; set; }
}

