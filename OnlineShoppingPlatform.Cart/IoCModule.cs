using Microsoft.Extensions.DependencyInjection;
using OnlineShoppingPlatform.Carts.Repositories;
using OnlineShoppingPlatform.Carts.Repositories.Interfaces;
using OnlineShoppingPlatform.Carts.Services;
using OnlineShoppingPlatform.Carts.Services.Interfaces;

namespace OnlineShoppingPlatform.Carts
{
    public static class IoCModule
    {
        public static IServiceCollection AddCartsModule(this IServiceCollection services)
        {
            services.AddScoped<ICartService, CartService>();
            services.AddScoped<ICartRepository, CartRepository>();

            return services;
        }
    }
}
