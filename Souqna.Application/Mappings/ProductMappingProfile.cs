using AutoMapper;
using Souqna.Application.DTOs;
using Souqna.Domin.Entities;

namespace Souqna.Application.Mappings
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(x => x.CategoryName, opt => opt.MapFrom(src => src.Category.Name));
            CreateMap<AddProductDto, Product>()
                .ForMember(i => i.Photos, opt => opt.Ignore())
                .ReverseMap();
            CreateMap<UpdateProductDto, Product>()
                .ForMember(i => i.Photos, opt => opt.Ignore())
                .ReverseMap();

            CreateMap<Photo, PhotoDto>().ReverseMap();
        }
    }
}
