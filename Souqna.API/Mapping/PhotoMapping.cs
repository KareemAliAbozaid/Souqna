using AutoMapper;
using Souqna.Domin.DTOs;
using Souqna.Domin.Entities;

namespace Souqna.API.Mapping
{
    public class PhotoMapping:Profile
    {
        public PhotoMapping()
        {
            CreateMap<Photo, PhotoDto>().ReverseMap();
        }
    }
}
