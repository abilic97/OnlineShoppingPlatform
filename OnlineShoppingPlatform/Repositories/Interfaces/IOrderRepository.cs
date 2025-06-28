using OnlineShoppingPlatform.Data.Entities;

namespace OnlineShoppingPlatform.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
    }
}
