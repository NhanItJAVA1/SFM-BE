using AutoMapper;
using SFM_BE.DTOs.Users;
using SFM_BE.Entities;

namespace SFM_BE.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<RegisterUserDto, User>();
        CreateMap<UpdateUserDto, User>();
        CreateMap<User, UserResponseDto>()
            .ForMember(
                dest => dest.Role,
                opt => opt.MapFrom(src => src.Role.Name.ToString()));
    }
}
