using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Auths;
using ChatApp.Dtos.Models.Users;

namespace ChatApp.Services.IServices
{
    public interface IUserService
    {
        Task<BaseResponseDto<bool>> Insert(InsertUserDto insertUserDto);

        Task<BaseResponseDto<ChangePasswordResponseDto>> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto);

        Task<BaseResponseDto<bool>> SendAccountConfirmation();

        Task<BaseResponseDto<LoginResponseDto>> ConfirmAccount(string token);

        Task<UserDto> Get(long userId);
    }
}
