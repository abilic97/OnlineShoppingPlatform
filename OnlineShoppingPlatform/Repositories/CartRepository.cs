using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Data;
using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ShoppingDbContext _context;
        public CartRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<Cart> GetByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .ToListAsync();
        }

        public async Task AddAsync(Cart cart)
        {
            await _context.Carts.AddAsync(cart);
        }

        public void Update(Cart cart)
        {
            _context.Carts.Update(cart);
        }

        public void Delete(Cart cart)
        {
            _context.Carts.Remove(cart);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
