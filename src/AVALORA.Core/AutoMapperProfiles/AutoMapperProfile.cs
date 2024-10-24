using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Domain.Models.ViewModels;
using AVALORA.Core.Dto.CartItemDtos;
using AVALORA.Core.Dto.CategoryDtos;
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

		// CartItem
		CreateMap<CartItem, CartItemResponse>().ReverseMap();
		CreateMap<CartItemAddRequest, CartItem>();
		CreateMap<CartItemResponse, CartItemUpdateRequest>();
		CreateMap<CartItemUpdateRequest, CartItem>();
    }
}

