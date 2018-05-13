using System.Linq;
using AutoMapper;
using myDotnetApp.API.Dtos;
using myDotnetApp.API.Model;

namespace myDotnetApp.API.Helpers
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<User, UserForListDtos>()
            .ForMember(dest => dest.PhotoUrl, opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<User, UserForDetailedDtos>()
            .ForMember(dest => dest.PhotoUrl, opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo, PhotosForDetailedDtos>();
            CreateMap<UserForUpdateDtos, User>();
        }
    }
}