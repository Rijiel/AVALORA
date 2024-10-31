using AVALORA.Core.Domain.Models;

namespace AVALORA.Web.Extensions;

/// <summary>
/// Extension methods for <see cref="OrderSummaryItem"/>
/// </summary>
public static class OrderSummaryItemExtension
{
	/// <summary>
	/// Populate the total price of an <see cref="OrderSummaryItem"/>
	/// </summary>
	/// <param name="item">The <see cref="OrderSummaryItem"/> to get the total price of.</param>
	/// <returns></returns>
	public static void GetTotalPrice(this OrderSummaryItem item)
	{
		if (item.Product != null)
		{
			item.TotalPrice = item.Product.Price * item.Count;
		}
	}
}
