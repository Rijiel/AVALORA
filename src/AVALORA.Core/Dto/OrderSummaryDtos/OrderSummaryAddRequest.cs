using AVALORA.Core.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.OrderSummaryDtos;

public class OrderSummaryAddRequest
{
	public int? OrderHeaderId { get; set; }

	public ICollection<OrderSummaryItem>? OrderSummaryItems { get; set; }

	[Required]
	[DataType(DataType.Currency)]
	public double TotalPrice { get; set; }
}

