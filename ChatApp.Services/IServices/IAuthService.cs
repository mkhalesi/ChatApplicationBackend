using System.IdentityModel.Tokens.Jwt;
using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Auths;
using ChatApp.Dtos.Models.Users;

namespace ChatApp.Services.IServices
{
    public interface IAuthService
    {
        Task<BaseResponseDto<LoginResponseDto>> Login(LoginRequestDto loginRequestDto);

        Task<BaseResponseDto<RegisterDto>> Register(RegisterDto registerDto);

        Task<BaseResponseDto<UserDto>> GetUserForAuthorization(long userId);

        Task<BaseResponseDto<LoginResponseDto>> GoogleLogin(GoogleLoginRequestDto loginRequestDto);

        Task<BaseResponseDto<bool>> ForgotPassword(string email);

        Task<(JwtSecurityToken token, UserDto user)> VerifyAuthToken(string token);
    }
}
    