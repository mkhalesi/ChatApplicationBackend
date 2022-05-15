using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Auths;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ChatApp.Api.Controllers
{
    [AllowAnonymous]
    [Route("api/auth")]
    public class AuthController : BaseApiController
    {
        private readonly ILogger<AuthController> _logger;
        private readonly IAuthService _authService;

        public AuthController(
            ILogger<AuthController> logger,
            IAuthService authService)
        {
            _logger = logger;
            _authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public async Task<BaseResponseDto<LoginResponseDto>> Login(LoginRequestDto loginRequestDto)
        {
            try
            {
                return await _authService.Login(loginRequestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Login error: {ex}");
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

        [HttpPost]
        [Route("register")]
        public async Task<BaseResponseDto<LoginResponseDto>> Register(RegisterDto registerDto)
        {
            try
            {
                return await _authService.Register(registerDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Register error: {ex}");
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

        [HttpPost]
        [Route("google-login")]
        public async Task<BaseResponseDto<LoginResponseDto>> GoogleLogin(GoogleLoginRequestDto loginRequestDto)
        {
            try
            {
                return await _authService.GoogleLogin(loginRequestDto);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Google Login error: {ex}");
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

        [HttpPost]
        [Route("forgot-password/{email}")]
        public async Task<BaseResponseDto<bool>> ForgotPassword(string email)
        {
            try
            {
                return await _authService.ForgotPassword(email);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Forgot password error: {ex}");
                return new BaseResponseDto<bool>()
                    .GenerateGeneralFailedResponse(ex.ToString());
            }
        }

    }
}
