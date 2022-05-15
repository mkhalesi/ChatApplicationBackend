using ChatApp.DataAccess.UoW;
using ChatApp.Services.IServices;
using ChatApp.Utilities.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace ChatApp.Api.Filters
{
    public class AuthFilter : IAsyncAuthorizationFilter
    {

        private readonly ILogger<AuthFilter> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAuthService _authService;

        public AuthFilter(ILogger<AuthFilter> logger, IUnitOfWork unitOfWork, IAuthService authService)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _authService = authService;
        }

        public async Task OnAuthorizationAsync(AuthorizationFilterContext context)
        {
            if (context.ActionDescriptor.EndpointMetadata.Any(item => item is AllowAnonymousAttribute))
            {
                return;
            }

            try
            {
                var authorization = context.HttpContext.Request
                    .Headers[RequestKeys.AuthorizationHeader]
                    .ToString()
                    .Split();
                if (authorization.Length != 2 || authorization[0] != GlobalConstants.AuthSchema)
                {
                    context.Result = new UnauthorizedResult();
                    return;
                }

                var (token, user) = await _authService.VerifyAuthToken(authorization[1]);

                var isGoogleLogin = Convert.ToBoolean(token.Claims
                    .First(x => x.Type == UserClaimTypes.IsGoogleLogin)
                    .Value);

                context.HttpContext.Items[RequestKeys.UserId] = user.Id;
                context.HttpContext.Items[RequestKeys.UserEmail] = user.Email;
                context.HttpContext.Items[RequestKeys.UserRole] = user.Role;
                context.HttpContext.Items[RequestKeys.IsGoogleLogin] = isGoogleLogin.ToString();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Auth filter error: {ex}");
                context.Result = new UnauthorizedResult();
            }
        }
    }
}
