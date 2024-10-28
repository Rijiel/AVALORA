using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ProductDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IProductService : IGenericService<Product, ProductAddRequest, ProductUpdateRequest, ProductResponse>
{
	/// <summary>
	/// Retrieves the total rating for a product asynchronously.
	/// </summary>
	/// <param name="id">The ID of the product.</param>
	/// <param name="cancellationToken">The cancellation token. Defaults to <see cref="CancellationToken.None"/>.</param>
	/// <returns>A task that represents the asynchronous operation. The task result contains the total rating as a decimal value.</returns>
	Task<decimal> GetTotalRatingAsync(int? id, CancellationToken cancellationToken = default);
}

