using OnlineShoppingPlatform.Repositories;
using OnlineShoppingPlatform.Repositories.Interfaces;
using OnlineShoppingPlatform.Services;
using OnlineShoppingPlatform.Services.Interfaces;

namespace OnlineShoppingPlatform
{
    public static class IocModule
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ICartRepository, CartRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICartService, CartService>();

            return services;
        }

        public static IServiceCollection AddCustomDependencies(this IServiceCollection services)
        {
            return services
                .AddRepositories()
                .AddServices();
        }
    }
}
