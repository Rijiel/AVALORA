using AVALORA.Core.Domain.Models;

namespace AVALORA.Core.Dto.OrderSummaryDtos;

public class OrderSummaryResponse
{
	public int Id { get; set; }
	public int OrderHeaderId { get; set; }
	public ICollection<OrderSummaryItem> OrderSummaryItems { get; set; } = null!;
	public double TotalPrice { get; set; }
}
