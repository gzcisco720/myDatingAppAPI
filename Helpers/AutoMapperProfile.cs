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
            CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<User, UserForDetailedDto>()
            .ForMember(dest => dest.PhotoUrl, opt =>{
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
            });
            CreateMap<Photo, PhotosForDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<UserForRegisterDto, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap();
            CreateMap<Message, MessageForReturnDto>()
            .ForMember(m => m.SenderPhotoUrl, opt => opt.MapFrom(u=>u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
            .ForMember(m => m.RecipientPhotoUrl, opt => opt.MapFrom(u=>u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));
        }
    }
}