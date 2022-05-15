using System.IdentityModel.Tokens.Jwt;
using ChatApp.Dtos.Models.Users;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
using Microsoft.AspNetCore.SignalR;

namespace ChatApp.Api.Filters
{
    public class HubAuthFilter : IHubFilter
    {
        private readonly IAuthService _authService;

        public HubAuthFilter(IAuthService authService)
        {
            _authService = authService;
        }

        public async Task OnConnectedAsync(HubLifetimeContext context, Func<HubLifetimeContext, Task> next)
        {
            await Authorize(context.Context.GetHttpContext() ?? throw new InvalidOperationException());
            await next(context);
        }

        public Task OnDisconnectedAsync(HubLifetimeContext context, Exception exception, Func<HubLifetimeContext, Exception, Task> next)
        {
            return next(context, exception);
        }

        public async ValueTask<object> InvokeMethodAsync(HubInvocationContext invocationContext, Func<HubInvocationContext, ValueTask<object>> next)
        {
            return await next(invocationContext);
        }

        private async Task Authorize(HttpContext context)
        {
            JwtSecurityToken token = new();
            UserDto user = new();

            try
            {
                var (jwtSecurityToken, userDto) =
                    await _authService.VerifyAuthToken(context.Request.Query[RequestKeys.AccessToken].ToString());

                token = jwtSecurityToken;
                user = userDto;
            }
            catch (Exception e)
            {
                throw new HubException(ErrorCodes.Unauthorized);
            }

            context.Items[RequestKeys.UserId] = user.Id;
            context.Items[RequestKeys.UserEmail] = user.Email;
        }
    }
}
