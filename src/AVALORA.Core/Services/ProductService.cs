using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.ServiceContracts;

namespace AVALORA.Core.Services;

public class ProductService : GenericService<Product, ProductAddRequest, ProductUpdateRequest, ProductResponse>, 
    IProductService
{
	public ProductService(IProductRepository repository, IMapper mapper, IUnitOfWork unitOfWork) 
        : base(repository, mapper, unitOfWork)
	{
	}

	public async Task<decimal> GetTotalRatingAsync(int? id, CancellationToken cancellationToken = default)
	{
		decimal totalRating = 0;

		ProductResponse? productResponse = await GetByIdAsync(id, includes: nameof(ProductResponse.ProductReviews), 
			cancellationToken: cancellationToken);

		if (productResponse != null)
		{
			if (productResponse.ProductReviews?.Count > 0)
				totalRating = (decimal)productResponse.ProductReviews.Average(p => p.Rating);
		}

		return totalRating;
	}
}

