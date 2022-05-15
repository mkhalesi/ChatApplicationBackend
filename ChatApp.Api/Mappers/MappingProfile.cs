using AutoMapper;

namespace ChatApp.Api.Mappers
{
    public class MappingProfile: Profile
    {
        public MappingProfile()
        {
            this.CreateUserMap();
        }
    }
}
