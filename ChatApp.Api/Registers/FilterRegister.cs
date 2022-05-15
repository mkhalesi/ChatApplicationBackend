using ChatApp.Api.Filters;

namespace ChatApp.Api.Registers
{
    public static class FilterRegister
    {
        public static void AddFilters(this IServiceCollection services)
        {
            services.AddScoped<AuthFilter>();
        }
    }
}
