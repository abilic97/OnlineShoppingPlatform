using Microsoft.Extensions.DependencyInjection;
using OnlineShoppingPlatform.Users.Repositories;
using OnlineShoppingPlatform.Users.Repositories.Interfaces;
using OnlineShoppingPlatform.Users.Services;
using OnlineShoppingPlatform.Users.Services.Interfaces;

namespace OnlineShoppingPlatform.Users
{
    public static class IoCModule
    {
        public static IServiceCollection AddUsersModule(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRepository, UserRepository>();

            return services;
        }
    }
}
