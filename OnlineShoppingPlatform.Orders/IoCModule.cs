using Microsoft.Extensions.DependencyInjection;
using OnlineShoppingPlatform.Orders.Factories;
using OnlineShoppingPlatform.Orders.Factories.Interfaces;
using OnlineShoppingPlatform.Orders.Repositories;
using OnlineShoppingPlatform.Orders.Repositories.Interfaces;
using OnlineShoppingPlatform.Orders.Services;
using OnlineShoppingPlatform.Orders.Services.Interfaces;

namespace OnlineShoppingPlatform.Orders
{
    public static class IoCModule
    {
        public static IServiceCollection AddOrdersModule(this IServiceCollection services)
        {
            services.AddScoped<IOrderService, OrderService>();
            services.AddScoped<IOrderFactory, OrderFactory>();
            services.AddScoped<IOrderRepository, OrderRepository>();

            return services;
        }
    }
}
