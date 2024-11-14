using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductReviewDtos;
using AVALORA.Core.ServiceContracts;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;
using Microsoft.AspNetCore.Http;
using System.Threading;

namespace AVALORA.Core.Services.FacadeServices;

public class PagerFacade : BaseFacade<PagerFacade>, IPagerFacade
{
	public PagerFacade(IServiceProvider serviceProvider) : base(serviceProvider)
	{
	}

	public async Task<List<ProductReviewResponse>> GetPagedProductReviews(int page, int pageSize, int productId, 
		CancellationToken cancellationToken = default)
	{
		List<ProductReviewResponse> productReviewResponses = await ServiceUnitOfWork.ProductReviewService
		.GetAllAsync(p => p.ProductId == productId, cancellationToken: cancellationToken);

		// Sort by comment length -> rating -> date posted
		productReviewResponses = productReviewResponses
			.OrderByDescending(p => p.Comment?.Length)
			.ThenByDescending(p => p.Rating)
			.ThenByDescending(p => p.DatePosted.Date).ToList();

		// Only show 3 most recent product reviews
		List<ProductReviewResponse> pagedProductReviewResponses = ServiceUnitOfWork.PagerService
			.GetPagedItems(productReviewResponses, page, pageSize: pageSize);

		return pagedProductReviewResponses;
	}
}
