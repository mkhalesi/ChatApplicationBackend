using AutoMapper;
using ChatApp.DataAccess.UoW;
using ChatApp.Dtos.Common;
using ChatApp.Dtos.Models.Auths;
using ChatApp.Dtos.Models.Users;
using ChatApp.Entities.Models.Access;
using ChatApp.Entities.Models.User;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
using ChatApp.Utilities.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace ChatApp.Services.Services
{
    public class UserService : IUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;

        public UserService(IUnitOfWork unitOfWork, IMapper mapper, IHttpContextAccessor contextAccessor, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _httpContextAccessor = contextAccessor;
            _configuration = configuration;
        }

        public async Task<BaseResponseDto<bool>> Insert(InsertUserDto insertUserDto)
        {
            if (insertUserDto == null)
                return new BaseResponseDto<bool>().GenerateFailedResponse(ErrorCodes.BadRequest);

            if (string.IsNullOrEmpty(insertUserDto.Email) ||
                string.IsNullOrEmpty(insertUserDto.Password))
            {
                return new BaseResponseDto<bool>().GenerateFailedResponse(ErrorCodes.BadRequest);
            }
            var userRepository = _unitOfWork.GetRepository<User>();
            var userRoleRepository = _unitOfWork.GetRepository<UserRole>();

            var existed = await userRepository.GetQuery().AnyAsync(p => p.Email.ToLower().Trim() == insertUserDto.Email.ToLower().Trim());
            if (existed)
                return new BaseResponseDto<bool>().GenerateFailedResponse(ErrorCodes.EmailExists);

            var user = _mapper.Map<User>(insertUserDto);
            user.Password = user.Password.HashMd5();

            await userRepository.AddEntity(user);
            await userRepository.SaveChanges();

            var roleFromDb = await userRoleRepository.GetQuery().FirstOrDefaultAsync(p => p.Role.Name == UserRoleName.UserRole);
            if (roleFromDb != null)
            {
                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = roleFromDb.RoleId
                };

                await userRoleRepository.AddEntity(newUserRole);
                await userRoleRepository.SaveChanges();
            }

            return new BaseResponseDto<bool>().GenerateSuccessResponse(true);
        }

        public async Task<BaseResponseDto<ChangePasswordResponseDto>> ChangePassword(ChangePasswordRequestDto changePasswordRequestDto)
        {
            if (changePasswordRequestDto == null)
            {
                return new BaseResponseDto<ChangePasswordResponseDto>().GenerateFailedResponse(ErrorCodes.BadRequest);
            }

            if (string.IsNullOrEmpty(changePasswordRequestDto.NewPassword))
            {
                return new BaseResponseDto<ChangePasswordResponseDto>().GenerateFailedResponse(ErrorCodes.BadRequest);
            }

            if (changePasswordRequestDto.CurrentPassword == changePasswordRequestDto.NewPassword)
            {
                return new BaseResponseDto<ChangePasswordResponseDto>().GenerateFailedResponse(ErrorCodes.SameNewPassword);
            }

            var hashCurrentPassword = changePasswordRequestDto.CurrentPassword.HashMd5();

            var userId = _httpContextAccessor.HttpContext.UserId();

            var userRepo = _unitOfWork.GetRepository<User>();
            var user = await userRepo.GetQuery().FirstOrDefaultAsync(p => p.Id == int.Parse(userId));
            if (user == null)
            {
                return new BaseResponseDto<ChangePasswordResponseDto>().GenerateFailedResponse(ErrorCodes.NotFound);
            }

            if (!user.IsConfirmed)
            {
                return new BaseResponseDto<ChangePasswordResponseDto>().GenerateFailedResponse(ErrorCodes.AccountHasNotBeenConfirmed);
            }

            if (!_httpContextAccessor.HttpContext.IsGoogleLogin() && user.Password != hashCurrentPassword)
            {
                return new BaseResponseDto<ChangePasswordResponseDto>().GenerateFailedResponse(ErrorCodes.IncorrectCurrentPassword);
            }

            var hashNewPassword = changePasswordRequestDto.NewPassword.HashMd5();
            user.Password = hashNewPassword;

            userRepo.EditEntity(user);
            await userRepo.SaveChanges();

            var jwtSetting = GetJwtSetting();

            return new BaseResponseDto<ChangePasswordResponseDto>().GenerateSuccessResponse(new ChangePasswordResponseDto
            {
                Token = user.GenerateAccessToken(jwtSetting)
            });
        }

        public async Task<BaseResponseDto<bool>> SendAccountConfirmation()
        {
            var userId = _httpContextAccessor.HttpContext.UserId();
            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.GetQuery().FirstOrDefaultAsync(p => p.Id == int.Parse(userId));
            if (user == null)
            {
                return new BaseResponseDto<bool>().GenerateFailedResponse(ErrorCodes.NotFound);
            }

            if (user.IsConfirmed)
            {
                return new BaseResponseDto<bool>().GenerateSuccessResponse(true);
            }

            if (string.IsNullOrEmpty(user.ConfirmationToken))
            {
                user.ConfirmationToken = Guid.NewGuid().ToString().HashMd5();
                userRepo.EditEntity(user);
                await userRepo.SaveChanges();
            }

            /*var result = await _emailService.Send(new EmailDto
            {
                Title = EmailTemplates.ConfirmAccountTitle,
                Address = user.Email,
                Content = EmailTemplates.ConfirmAccountBody
                    .Replace("#name#", user.FirstName)
                    .Replace("#link#", $"{_configuration[AppSettingKeys.FrontEndHost]}/confirm-account/{user.ConfirmationToken}")
            });*/

            return new BaseResponseDto<bool>().GenerateSuccessResponse(true);
        }

        public async Task<BaseResponseDto<LoginResponseDto>> ConfirmAccount(string token)
        {
            var userRepo = _unitOfWork.GetRepository<User>();

            var user = await userRepo.GetQuery()
                .FirstOrDefaultAsync(p => p.ConfirmationToken.ToLower().TrimEnd() == token.ToLower().Trim());
            if (user == null)
            {
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateFailedResponse(ErrorCodes.NotFound);
            }

            var jwtSetting = GetJwtSetting();

            LoginResponseDto loginResponseDto;

            if (user.IsConfirmed)
            {
                loginResponseDto = new LoginResponseDto
                {
                    User = _mapper.Map<UserResponseDto>(user),
                    Token = user.GenerateAccessToken(jwtSetting, true)
                };

                return new BaseResponseDto<LoginResponseDto>().GenerateSuccessResponse(loginResponseDto);
            }

            user.IsConfirmed = true;
            user.ConfirmationToken = null;

            userRepo.EditEntity(user);
            await userRepo.SaveChanges();

            loginResponseDto = new LoginResponseDto
            {
                User = _mapper.Map<UserResponseDto>(user),
                Token = user.GenerateAccessToken(jwtSetting)
            };

            return new BaseResponseDto<LoginResponseDto>().GenerateSuccessResponse(loginResponseDto);
        }

        public async Task<UserDto> Get(long userId)
        {
            var userRepository = _unitOfWork.GetRepository<User>();
            var userFromDb = await userRepository.GetQuery().FirstOrDefaultAsync(p => p.Id == userId);

            return userFromDb != null ? _mapper.Map<UserDto>(userFromDb) : null;
        }

        #region helpers

        private JwtSettingDto GetJwtSetting()
        {
            var jwtSetting = new JwtSettingDto();
            var result = _configuration
                .GetSection(AppSettingKeys.JwtSettingSection)
                .GetChildren();

            foreach (var section in result)
            {
                if (section.Key == "ExpiredInDays")
                    jwtSetting.ExpiredInDays = int.Parse(section.Value);
            }

            return jwtSetting;
        }

        #endregion
    }
}
