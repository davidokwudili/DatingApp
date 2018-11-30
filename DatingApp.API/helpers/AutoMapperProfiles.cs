using System.Linq;
using AutoMapper;
using DatingApp.Api.DTOs;
using DatingApp.API.DTOs;
using DatingApp.DTOs;
using DatingApp.Models;

namespace DatingApp.API.helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserForListDto>()
                    .ForMember(dest => dest.PhotoUrl, opt =>
                    {
                        opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                    })
                    .ForMember(dest => dest.Age, opt =>
                    {
                        opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                    });

            CreateMap<User, UserForDetailedDto>()
                    .ForMember(dest => dest.PhotoUrl, opt =>
                    {
                        opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                    })
                    .ForMember(dest => dest.Age, opt =>
                    {
                        opt.ResolveUsing(d => d.DateOfBirth.CalculateAge());
                    });

            CreateMap<User, PhotosForUserDetailedDto>();
            CreateMap<UserForUpdateDto, User>();
            CreateMap<Photo, PhotoForReturnDto>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<DtoUserForRegister, User>();
            CreateMap<MessageForCreationDto, Message>().ReverseMap(); //to reverse in order to go the other way round, to support both
            CreateMap<Message, MessageToReturnDto>()
                    .ForMember(dest => dest.SenderPhotoUrl, opt =>
                    {
                        opt.MapFrom(src => src.Sender.Photos.FirstOrDefault(p => p.IsMain).Url);
                    })
                    .ForMember(dest => dest.RecipientPhotoUrl, opt =>
                    {
                        opt.MapFrom(src => src.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url);
                    });
        }
    }
}