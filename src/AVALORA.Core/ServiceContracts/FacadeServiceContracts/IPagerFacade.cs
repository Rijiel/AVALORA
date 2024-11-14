using AVALORA.Core.Dto.ProductReviewDtos;

namespace AVALORA.Core.ServiceContracts.FacadeServiceContracts;

public interface IPagerFacade
{
	/// <summary>
	/// Retrieves a paged list of product reviews.
	/// </summary>
	/// <param name="page">The page number to retrieve.</param>
	/// <param name="pageSize">The number of items to retrieve per page.</param>
	/// <param name="productId">The ID of the product to retrieve reviews for.</param>
	/// <param name="cancellationToken">A cancellation token that can be used by other objects or threads to receive notice of cancellation.</param>
	/// <returns>A task representing the asynchronous operation. Returns a list of product review responses.</returns>
	Task<List<ProductReviewResponse>> GetPagedProductReviews(int page, int pageSize, int productId,
		CancellationToken cancellationToken = default);
}
