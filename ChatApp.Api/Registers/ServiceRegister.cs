using ChatApp.DataAccess.Repository;
using ChatApp.DataAccess.UoW;
using ChatApp.Services.IServices;
using ChatApp.Services.Services;

namespace ChatApp.Api.Registers
{
    public static class ServiceRegister
    {
        public static void AddServices(this IServiceCollection services)
        {
            /*services.AddScoped(typeof(IRepository<>), typeof(Repository<>));*/
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IChatService, ChatService>();

            services.AddScoped<IUnitOfWork, UnitOfWork>();
        }
    }
}
