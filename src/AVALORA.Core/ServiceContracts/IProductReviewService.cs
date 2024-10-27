using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ProductReviewDtos;

namespace AVALORA.Core.ServiceContracts;

public interface IProductReviewService : IGenericService<ProductReview, ProductReviewAddRequest, ProductReviewUpdateRequest, ProductReviewResponse>
{
}

