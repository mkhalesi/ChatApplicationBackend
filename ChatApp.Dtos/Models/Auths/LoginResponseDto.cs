using ChatApp.Dtos.Models.Users;

namespace ChatApp.Dtos.Models.Auths
{
    public class LoginResponseDto
    {
        public string Token { get; set; }

        public UserResponseDto User { get; set; }
    }
}
