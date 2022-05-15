using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ChatApp.Dtos.Models.Auths;
using ChatApp.Entities.Models.User;
using ChatApp.Utilities.Constants;
using Microsoft.IdentityModel.Tokens;

namespace ChatApp.Utilities.Extensions
{
    public static class StringExtensions
    {
        public static string HashMd5(this string source)
        {
            if (string.IsNullOrEmpty(source))
            {
                throw new ArgumentNullException(nameof(source));
            }

            using var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(source);
            var hashBytes = md5.ComputeHash(inputBytes);

            var sb = new StringBuilder();
            foreach (var hashByte in hashBytes)
            {
                sb.Append(hashByte.ToString("X2"));
            }
            return sb.ToString();
        }

        public static string GenerateAccessToken(this User user, JwtSettingDto jwtSettingDto, bool isGoogleLogin = false)
        {
            if (jwtSettingDto == null)
            {
                throw new ArgumentNullException(nameof(jwtSettingDto));
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(!isGoogleLogin
                ? user.Password
                : user.GooglePassword);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(UserClaimTypes.UserId, user.Id.ToString()),
                    new Claim(UserClaimTypes.Email, user.Email),
                    new Claim(UserClaimTypes.IsGoogleLogin, isGoogleLogin.ToString()),
                }),
                Expires = DateTime.Now.AddDays(jwtSettingDto.ExpiredInDays),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public static string GenerateRandomPassword(this int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
                .Select(s => s[new Random().Next(s.Length)]).ToArray());
        }
    }
}
