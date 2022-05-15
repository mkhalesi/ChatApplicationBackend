using ChatApp.Dtos.Models.Auths;
using ChatApp.Dtos.Models.Users;
using ChatApp.Entities.Models.User;

namespace ChatApp.Api.Mappers
{
    public static class UserMappingProfile
    {
        public static void CreateUserMap(this MappingProfile mappingProfile)
        {
            mappingProfile.CreateMap<User, UserDto>().ReverseMap();
            mappingProfile.CreateMap<InsertUserDto, UserDto>();
            mappingProfile.CreateMap<RegisterDto, UserDto>();
            mappingProfile.CreateMap<User, UserResponseDto>();
        }
    }
}
