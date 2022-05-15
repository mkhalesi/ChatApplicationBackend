using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Auths;
using ChatApp.Dtos.Models.Users;
using ChatApp.Entities.Models.Access;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [Route("api/users")]
    public class UserController : BaseApiController
    {
        private readonly ILogger<UserController> _logger;
        private readonly IUserService _userService;

        public UserController(
            ILogger<UserController> logger,
            IUserService userService)
        {
            _logger = logger;
            _userService = userService;
        }

        [HttpPost]
        [Route("insert")]
        ///*[TypeFilter(typeof(RoleFilter), Arguments = new object[] { new[] { UserRole.Admin } })]*/
        public async Task<BaseResponseDto<bool>> Insert(InsertUserDto insertUserDto)
        {
            try
            {
                return await _userService.Insert(insertUserDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Insert user error: {ex}");
                return new BaseResponseDto<bool>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

        [HttpPut]
        [Route("change-password")]
        public async Task<BaseResponseDto<ChangePasswordResponseDto>> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
        {
            try
            {
                return await _userService.ChangePassword(changePasswordRequestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Change password error: {ex}");
                return new BaseResponseDto<ChangePasswordResponseDto>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

        [HttpPost]
        [Route("send-confirmation")]
        public async Task<BaseResponseDto<bool>> SendAccountConfirmation()
        {
            try
            {
                return await _userService.SendAccountConfirmation();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Send confirmation email error: {ex}");
                return new BaseResponseDto<bool>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

        [HttpPost]
        [Route("confirm-account/{token}")]
        [AllowAnonymous]
        public async Task<BaseResponseDto<LoginResponseDto>> ConfirmAccount(string token)
        {
            try
            {
                return await _userService.ConfirmAccount(token);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Confirm account error: {ex}");
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }
    }
}
