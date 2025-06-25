using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Data;
using OnlineShoppingPlatform.Domain.DTO;
using OnlineShoppingPlatform.Domain.EntityMappers;
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

        public async Task<CartDto> GetByIdAsync(int cartId)
        {
            var cartEntity = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cartEntity == null)
                return null!; // or handle not found case as you prefer

            // Map entity to DTO before returning
            var cartDto = cartEntity.ToDto();
            return cartDto;
        }

        public async Task AddAsync(CartDto cart)
        {
            var cartTb = cart.ToEntity();
            await _context.Carts.AddAsync(cartTb);
        }

        public void Update(CartDto cart)
        {
            var cartTb = cart.ToEntity();
            _context.Carts.Update(cartTb);
        }

        public void Delete(CartDto cart)
        {
            var cartTb = cart.ToEntity();
            _context.Carts.Remove(cartTb);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
    }
}
