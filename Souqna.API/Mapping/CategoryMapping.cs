using AutoMapper;
using Souqna.Domin.DTOs;
using Souqna.Domin.Entities;

public class CategoryMapping : Profile
{
    public CategoryMapping()
    {
        CreateMap<Category, CategoryDto>().ReverseMap();
        CreateMap<Category, UpdateCategoryDto>().ReverseMap().ForMember(dest => dest.Id, opt => opt.Ignore());
    }
}
