using AVALORA.Core.Domain.Models;
using AVALORA.Core.Enums;

namespace AVALORA.Core.Dto.OrderSummaryItemDtos;

public class OrderSummaryItemResponse
{
	public int Id { get; set; }
	public int ProductId { get; set; }
	public Product? Product { get; set; }
	public int Count { get; set; }
	public int OrderSummaryId { get; set; }
	public OrderSummary? OrderSummary { get; set; }
	public Color Color { get; set; }
}

