using Microsoft.Extensions.DependencyInjection;
using OnlineShoppingPlatform.Products.Repositories;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;
using OnlineShoppingPlatform.Products.Services;
using OnlineShoppingPlatform.Products.Services.Interfaces;

namespace OnlineShoppingPlatform.Products
{
    public static class IoCModule
    {
        public static IServiceCollection AddProductsModule(this IServiceCollection services)
        {
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductRepository, ProductRepository>();

            return services;
        }
    }
}
