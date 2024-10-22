using AutoMapper;
using AVALORA.Core.Domain.Models;
using AVALORA.Core.Dto.Category;

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
    }
}

