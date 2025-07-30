using OnlineShoppingPlatform.Carts.DTO;

namespace OnlineShoppingPlatform.Carts.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetOrCreateUserCartAsync(string userId);
        Task<bool> DeleteAsync(int cartId);
        Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem);
        Task<CartDto?> RemoveItemAsync(int cartId, int cartItemId);
    }
}
