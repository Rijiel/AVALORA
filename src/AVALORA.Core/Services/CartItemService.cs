using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.RepositoryContracts;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.ServiceContracts;
using System.Reflection;

namespace AVALORA.Core.Services;

public class CartItemService : GenericService<CartItem, CartItemAddRequest, CartItemUpdateRequest, CartItemResponse>,
    ICartItemService
{
	public CartItemService(ICartItemRepository repository, IMapper mapper, IUnitOfWork unitOfWork) 
        : base(repository, mapper, unitOfWork)
    {
	}

    public double GetTotalPrice(IEnumerable<CartItemResponse>? cartItemResponses)
    {
        if (cartItemResponses != null)
        {
            double totalPrice = 0;

            foreach (var cartItemResponse in cartItemResponses)
                totalPrice += (cartItemResponse.Product.Price * cartItemResponse.Count);

            return totalPrice;
        }        

        return 0;
    }
}

