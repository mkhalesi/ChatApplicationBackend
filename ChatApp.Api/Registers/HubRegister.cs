using ChatApp.Api.Hubs;

namespace ChatApp.Api.Registers
{
    public static class HubRegister
    {
        public static void AddHubs(this IServiceCollection services)
        {
            services.AddScoped<ChatHub>();
        }

        public static void MapSignalRHubs(this IEndpointRouteBuilder endpoints)
        {
            endpoints.MapHub<ChatHub>("/chat");
        }
    }
}
