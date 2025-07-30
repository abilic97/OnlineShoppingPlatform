using OnlineShoppingPlatform.Carts.DTO;

namespace OnlineShoppingPlatform.Carts.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartDto> GetByIdAsync(int cartId);
        Task AddAsync(CartDto cart);
        void Update(CartDto cart);
        void Delete(CartDto cart);
        Task<bool> SaveChangesAsync();
        Task<CartDto> GetByUserIdAsync(string userId);
        Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem);
        Task<CartDto?> RemoveItemAsync(int cartId, int cartItemId);
    }
}
