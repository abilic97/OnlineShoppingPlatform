using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Carts.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task AddAsync(CartDto cart);
        Task<bool> DeleteAsync(int cartId);
        Task<CartDto> GetByUserIdAsync(string userId);
        Task UpdateAsync(Cart cart);
        Task<Cart?> GetCartWithItemsAsync(int cartId);
    }
}
