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
          
            CreateMap<Photo, PhotoDto>();
        }
    }
}
