using OnlineShoppingPlatform.Carts.DTO;

namespace OnlineShoppingPlatform.Carts.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> CreateAsync(CartDto cart);
        Task<bool> DeleteAsync(int cartId);
        Task<CartDto> GetByUserIdAsync(string userId);
        Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem);
        Task<CartDto?> RemoveItemAsync(int cartId, int cartItemId);
    }
}
