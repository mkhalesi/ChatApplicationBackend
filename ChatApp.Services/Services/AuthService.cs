using System.IdentityModel.Tokens.Jwt;
using System.Text;
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
using Google.Apis.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Services.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        public AuthService(IUnitOfWork unitOfWork, IMapper mapper, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _configuration = configuration;
        }
        public async Task<BaseResponseDto<LoginResponseDto>> Login(LoginRequestDto loginRequestDto)
        {
            if (loginRequestDto == null)
            {
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateFailedResponse(ErrorCodes.BadRequest);
            }

            if (string.IsNullOrEmpty(loginRequestDto.Email) ||
                string.IsNullOrEmpty(loginRequestDto.Password))
            {
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateFailedResponse(ErrorCodes.InvalidCredential);
            }

            var userRepository = _unitOfWork.GetRepository<User>();
            var userFromDb = await userRepository.GetQuery()
                .FirstOrDefaultAsync(p => p.Email.ToLower().Trim() == loginRequestDto.Email.ToLower().Trim() &&
                                          p.Password == loginRequestDto.Password.HashMd5());

            if (userFromDb == null)
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateFailedResponse(ErrorCodes.NotFound, "user not found");

            return new BaseResponseDto<LoginResponseDto>().GenerateSuccessResponse(new LoginResponseDto()
            {
                User = _mapper.Map<UserResponseDto>(userFromDb),
                Token = userFromDb.GenerateAccessToken(GetJwtSetting(), false),
            });
        }

        public async Task<BaseResponseDto<LoginResponseDto>> Register(RegisterDto registerDto)
        {
            if (registerDto == null ||
                string.IsNullOrEmpty(registerDto.Email) ||
                string.IsNullOrEmpty(registerDto.Password))
            {
                return new BaseResponseDto<LoginResponseDto>().GenerateFailedResponse(ErrorCodes.BadRequest);
            }

            var userRepository = _unitOfWork.GetRepository<User>();
            var userEmailExist = await userRepository.GetQuery()
                .AnyAsync(p => p.Email.ToLower().Trim() == registerDto.Email.ToLower().TrimEnd());
            if (userEmailExist)
                return new BaseResponseDto<LoginResponseDto>().GenerateFailedResponse(ErrorCodes.EmailExists);

            var user = _mapper.Map<User>(registerDto);

            user.FirstName = user.FirstName.Trim();
            user.LastName = user.LastName.Trim();
            user.Email = user.Email.Trim();
            user.Password = user.Password.HashMd5();
            user.GooglePassword = user.Password.HashMd5();
            user.ConfirmationToken = Guid.NewGuid().ToString().HashMd5();

            await userRepository.AddEntity(user);
            await userRepository.SaveChanges();

            var userRoleRepository = _unitOfWork.GetRepository<UserRole>();
            var roleRepository = _unitOfWork.GetRepository<Role>();
            var roleFromDb = await roleRepository.GetQuery().FirstOrDefaultAsync(p => p.Name == UserRoleName.UserRole);
            if (roleFromDb != null)
            {
                var newUserRole = new UserRole()
                {
                    UserId = user.Id,
                    RoleId = roleFromDb.Id
                };

                await userRoleRepository.AddEntity(newUserRole);
                await userRoleRepository.SaveChanges();
            }

            return await Login(new LoginRequestDto
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            });
        }

        public async Task<BaseResponseDto<LoginResponseDto>> GoogleLogin(GoogleLoginRequestDto loginRequestDto)
        {
            if (loginRequestDto == null || string.IsNullOrEmpty(loginRequestDto.Token))
            {
                return new BaseResponseDto<LoginResponseDto>()
                    .GenerateFailedResponse(ErrorCodes.BadRequest);
            }

            var googleResult = await GoogleJsonWebSignature.ValidateAsync(loginRequestDto.Token);

            var jwtSetting = GetJwtSetting();
            var userRepository = _unitOfWork.GetRepository<User>();

            var user = await userRepository.GetQuery()
                .FirstOrDefaultAsync(p => p.Email.ToLower().Trim() == googleResult.Email.ToLower().Trim());

            if (user == null)
            {
                user = new User()
                {
                    Email = googleResult.Email,
                    FirstName = googleResult.GivenName,
                    LastName = googleResult.FamilyName,
                    GooglePassword = Guid.NewGuid().ToString().HashMd5()
                };

                await userRepository.AddEntity(user);
                await userRepository.SaveChanges();

                return new BaseResponseDto<LoginResponseDto>().GenerateSuccessResponse(new LoginResponseDto()
                {
                    User = _mapper.Map<UserResponseDto>(user),
                    Token = user.GenerateAccessToken(jwtSetting, true),
                });
            }

            return new BaseResponseDto<LoginResponseDto>().GenerateSuccessResponse(new LoginResponseDto()
            {
                User = _mapper.Map<UserResponseDto>(user),
                Token = user.GenerateAccessToken(jwtSetting, true),
            });
        }

        public Task<BaseResponseDto<bool>> ForgotPassword(string email)
        {
            throw new NotImplementedException();
        }

        public async Task<(JwtSecurityToken token, UserDto user)> VerifyAuthToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var tokenHandler = new JwtSecurityTokenHandler();

            var jwtToken = handler.ReadJwtToken(token);

            var userId = jwtToken.Claims
                .First(x => x.Type == UserClaimTypes.UserId)
                .Value;

            var user = await _unitOfWork
                .GetRepository<User>()
                .GetQuery().FirstOrDefaultAsync(p => p.Id == int.Parse(userId));

            if (user == null) throw new Exception("User is null");

            var isGoogleLogin = Convert.ToBoolean(jwtToken.Claims
                .First(x => x.Type == UserClaimTypes.IsGoogleLogin)
                .Value);

            var key = Encoding.ASCII.GetBytes(
                !isGoogleLogin
                    ? user.Password
                    : user.GooglePassword);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return (jwtToken, _mapper.Map<UserDto>(user));
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
