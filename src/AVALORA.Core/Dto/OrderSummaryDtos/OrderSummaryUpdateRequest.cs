using AVALORA.Core.Domain.Models;
using System.ComponentModel.DataAnnotations;

namespace AVALORA.Core.Dto.OrderSummaryDtos;

public class OrderSummaryUpdateRequest
{
	public int Id { get; set; }

	public ICollection<OrderSummaryItem>? OrderSummaryItems { get; set; }

	[Required]
	[DataType(DataType.Currency)]
	public double TotalPrice { get; set; }
}
