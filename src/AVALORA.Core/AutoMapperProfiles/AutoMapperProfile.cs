using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.ApplicationUserDtos;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.CategoryDtos;
using AVALORA.Core.Dto.OrderHeaderDtos;
using AVALORA.Core.Dto.OrderSummaryDtos;
using AVALORA.Core.Dto.OrderSummaryItemDtos;
using AVALORA.Core.Dto.ProductDtos;
using AVALORA.Core.Dto.ProductImageDtos;

namespace AVALORA.Core.AutoMapperProfiles;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Category
        CreateMap<Category, CategoryResponse>().ReverseMap();
        CreateMap<CategoryAddRequest, Category>();
        CreateMap<CategoryResponse, CategoryUpdateRequest>();
        CreateMap<CategoryUpdateRequest, Category>();

        // Product
		CreateMap<Product, ProductResponse>().ReverseMap();
		CreateMap<ProductAddRequest, Product>();
		CreateMap<ProductResponse, ProductUpdateRequest>();
		CreateMap<ProductUpdateRequest, Product>();

		// Product - ProductUpsertVM
		CreateMap<ProductResponse, ProductUpsertVM>().ReverseMap();
		CreateMap<ProductUpsertVM, ProductAddRequest>().ReverseMap();
		CreateMap<ProductUpsertVM, ProductUpdateRequest>().ReverseMap();

		// Product Image
		CreateMap<ProductImage, ProductImageResponse>().ReverseMap();
		CreateMap<ProductImageAddRequest, ProductImage>();
		CreateMap<ProductImageResponse, ProductImageUpdateRequest>();
		CreateMap<ProductImageUpdateRequest, ProductImage>();

		// OrderHeader
		CreateMap<OrderHeader, OrderHeaderResponse>().ReverseMap();
		CreateMap<OrderHeaderAddRequest, OrderHeader>();
		CreateMap<OrderHeaderResponse, OrderHeaderUpdateRequest>();
		CreateMap<OrderHeaderUpdateRequest, OrderHeader>();

		// OrderHeader - CheckoutVM
		CreateMap<OrderHeaderResponse, CheckoutVM>().ReverseMap();
		CreateMap<CheckoutVM, OrderHeaderAddRequest>().ReverseMap();
		CreateMap<CheckoutVM, OrderHeaderUpdateRequest>().ReverseMap();

		// OrderSummary
		CreateMap<OrderSummary, OrderSummaryResponse>().ReverseMap();
		CreateMap<OrderSummaryAddRequest, OrderSummary>();
		CreateMap<OrderSummaryResponse, OrderSummaryUpdateRequest>();
		CreateMap<OrderSummaryUpdateRequest, OrderSummary>();

		// OrderSummaryItem
		CreateMap<OrderSummaryItem, OrderSummaryItemResponse>().ReverseMap();
		CreateMap<OrderSummaryItemAddRequest, OrderSummaryItem>();
		CreateMap<OrderSummaryItemResponse, OrderSummaryItemUpdateRequest>();
		CreateMap<OrderSummaryItemUpdateRequest, OrderSummaryItem>();

		// OrderSummary - CheckoutVM
		CreateMap<OrderSummaryResponse, CheckoutVM>().ReverseMap();
		CreateMap<CheckoutVM, OrderSummaryAddRequest>().ReverseMap();
		CreateMap<CheckoutVM, OrderSummaryUpdateRequest>().ReverseMap();

		// CartItem
		CreateMap<CartItem, CartItemResponse>().ReverseMap();
		CreateMap<CartItemAddRequest, CartItem>();
		CreateMap<CartItemResponse, CartItemUpdateRequest>();
		CreateMap<CartItemUpdateRequest, CartItem>();	

		// CartItem - OrderSummaryItem
		CreateMap<CartItemResponse, OrderSummaryItem>().ReverseMap();
		CreateMap<CartItemResponse, OrderSummaryItemAddRequest>().ReverseMap();
		CreateMap<CartItemResponse, OrderSummaryItemUpdateRequest>().ReverseMap();
		CreateMap<OrderSummaryItem, CartItemAddRequest>().ReverseMap();
		CreateMap<OrderSummaryItem, CartItemUpdateRequest>().ReverseMap();
		
		// ApplicationUser
		CreateMap<ApplicationUser, ApplicationUserResponse>().ReverseMap();
		CreateMap<ApplicationUserAddRequest, ApplicationUser>();
		CreateMap<ApplicationUserResponse, ApplicationUserUpdateRequest>();
		CreateMap<ApplicationUserUpdateRequest, ApplicationUser>();

		// ApplicationUser - OrderHeader
		CreateMap<ApplicationUser, OrderHeaderResponse>().ReverseMap();
		CreateMap<ApplicationUserResponse, OrderHeader>();
		CreateMap<ApplicationUserResponse, OrderHeaderResponse>();
		CreateMap<ApplicationUserResponse, OrderHeaderAddRequest>()
			.ForMember(dest => dest.ApplicationUserId, opt => opt.MapFrom(src => src.Id));
		CreateMap<ApplicationUserResponse, OrderHeaderUpdateRequest>();
    }
}

