using OnlineShoppingPlatform.Data.Entities;

namespace OnlineShoppingPlatform.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart> GetByIdAsync(int cartId);
        Task<IEnumerable<Cart>> GetAllAsync();
        Task AddAsync(Cart cart);
        void Update(Cart cart);
        void Delete(Cart cart);
        Task<bool> SaveChangesAsync();
    }
}
