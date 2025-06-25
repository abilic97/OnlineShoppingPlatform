using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Domain.DTO;

namespace OnlineShoppingPlatform.Repositories.Interfaces
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
    }
}
