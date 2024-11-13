using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ProductReviewDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class ProductReviewService : GenericService<ProductReview, ProductReviewAddRequest, ProductReviewUpdateRequest, 
    ProductReviewResponse>, IProductReviewService
{
    public ProductReviewService(IProductReviewRepository repository, IMapper mapper, IUnitOfWork unitOfWork) 
        : base(repository, mapper, unitOfWork)
	{        
    }
}

