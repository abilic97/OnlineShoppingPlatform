using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Orders.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
    }
}
