using OnlineShoppingPlatform.Domain.DTO;

namespace OnlineShoppingPlatform.Services.Interfaces
{
    public interface IOrderService
    {
        Task<int> PlaceOrderAsync(CartDto cartDto);
    }
}
