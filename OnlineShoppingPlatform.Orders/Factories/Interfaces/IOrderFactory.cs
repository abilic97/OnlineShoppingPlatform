using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Orders.Factories.Interfaces
{
    public interface IOrderFactory
    {
        Order CreateOrder(CartDto cartDto, Dictionary<int, Product> productDict);
    }
}
