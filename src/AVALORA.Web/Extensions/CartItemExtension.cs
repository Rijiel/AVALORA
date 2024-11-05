using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.CartItemDtos;

namespace AVALORA.Web.Extensions;

/// <summary>
/// Extension methods for <see cref="CartItemResponse"/>
/// </summary>
public static class CartItemExtension
{
	/// <summary>
	/// Populate the total price of an <see cref="CartItemResponse"/>
	/// </summary>
	/// <param name="item">The <see cref="CartItemResponse"/> to get the total price of.</param>
	/// <returns></returns>
	public static void GetTotalPrice(this CartItemResponse item)
	{
		if (item.Product != null)
		{
			item.TotalPrice = item.Product.Price * item.Count;
		}
	}
}
