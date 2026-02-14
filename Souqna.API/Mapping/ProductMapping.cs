using AutoMapper;
using Souqna.Domin.DTOs;
using Souqna.Domin.Entities;

namespace Souqna.API.Mapping
{
    public class ProductMapping:Profile
    {
        public ProductMapping()
        {
         CreateMap<Product, ProductDto>().ForMember(x=>x.CategoryName,opt=>opt.MapFrom(src=>src.Category.Name)).ReverseMap();
         CreateMap<AddProductDto, Product>().ForMember(i=>i.Photos,opt=>opt.Ignore()).ReverseMap();
         CreateMap<UpdateProductDto, Product>().ForMember(i=>i.Photos,opt=>opt.Ignore()).ReverseMap();
        }
    }
}
