using api.DTOs;
using api.Entities;
using api.Extensions;
using AutoMapper;

namespace api.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<AppUser, MemberDto>().
                ForMember(dist => dist.PhotoUrl, opt => opt
                .MapFrom(src => src.Photos
                .FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dist => dist.Age, opt => opt
                .MapFrom(src => src.DateOfBirth.CalculateAge()));
          
            CreateMap<MemberUpdateDto, AppUser>();
            CreateMap<Photo, PhotoDto>();
            CreateMap<RegisterDTO, AppUser>();

            CreateMap<AppUser, LikeDto>().
              ForMember(dist => dist.PhotoUrl, opt => opt
              .MapFrom(src => src.Photos
              .FirstOrDefault(p => p.IsMain).Url))
              .ForMember(dist => dist.Age, opt => opt
              .MapFrom(src => src.DateOfBirth.CalculateAge()));

            CreateMap<Message, MessageDto>()
                .ForMember(s => s.SenderPhotoUrl, o => o.MapFrom(s => s.Sender.Photos.FirstOrDefault(s => s.IsMain).Url))
                .ForMember(s => s.RecipientPhotoUrl, o => o.MapFrom(s => s.Recipient.Photos.FirstOrDefault(s => s.IsMain).Url));

        }
    }
}
