using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Domain.DTO;

namespace OnlineShoppingPlatform.Services.Interfaces
{
    public interface ICartService
    {
        Task<CartDto> GetByIdAsync(int cartId);
        Task<CartDto> CreateAsync(CartDto cart);
        Task<CartDto> UpdateStatusAsync(int cartId, string newStatus);
        Task<bool> DeleteAsync(int cartId);
        Task<CartDto> RecalculateTotalsAsync(int cartId);
        Task<CartDto> GetByUserIdAsync(string userId);
        Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem);
    }
}
