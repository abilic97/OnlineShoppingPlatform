using OnlineShoppingPlatform.Data.Entities;

namespace OnlineShoppingPlatform.Services.Interfaces
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetAllAsync();
        Task<Cart> GetByIdAsync(int cartId);
        Task<Cart> CreateAsync(Cart cart);
        Task<Cart> UpdateStatusAsync(int cartId, string newStatus);
        Task<bool> DeleteAsync(int cartId);
        Task<Cart> RecalculateTotalsAsync(int cartId);
    }
}
