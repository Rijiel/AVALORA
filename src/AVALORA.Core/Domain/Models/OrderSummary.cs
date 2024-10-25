using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AVALORA.Core.Domain.Models;

public class OrderSummary
{
	public int Id { get; set; }

	[Required]
	public int OrderHeaderId { get; set; }

	[ForeignKey(nameof(OrderHeaderId))]
	public OrderHeader? OrderHeader { get; set; }

	public ICollection<OrderSummaryItem>? OrderSummaryItems { get; set; }

	[Required]
	[DataType(DataType.Currency)]
	public double TotalPrice { get; set; }
}

