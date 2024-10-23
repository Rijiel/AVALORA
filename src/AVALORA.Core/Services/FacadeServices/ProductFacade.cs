using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;
using AVALORA.Core.ServiceContracts.FacadeServiceContracts;

namespace AVALORA.Core.Services.FacadeServices;

public class ProductFacade : BaseFacade<ProductFacade>, IProductFacade
{
    public ProductFacade(IServiceProvider serviceProvider) : base(serviceProvider)
    {        
    }

    public async Task CreateProductAsync(ProductAddRequest productAddRequest)
    {
        ProductResponse productResponse = await ServiceUnitOfWork.ProductService.AddAsync(productAddRequest);
        ProductUpdateRequest productUpdateRequest = Mapper.Map<ProductUpdateRequest>(productResponse);

        // Check if there are any image files in the request
        if (productAddRequest.ImageFiles?.Count > 0)
        {
            List<ProductImageResponse> productImageResponses =
                await ServiceUnitOfWork.ProductImageService.CreateImagesAsync(productResponse.Id, productAddRequest.ImageFiles);
            productUpdateRequest.ProductImages = Mapper.Map<List<ProductImage>>(productImageResponses);
        }

        // Update the product to contain the images after creating them
        await ServiceUnitOfWork.ProductService.UpdateAsync(productUpdateRequest);
    }
}

