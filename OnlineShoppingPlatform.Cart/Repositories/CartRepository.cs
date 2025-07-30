using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Carts.Repositories.Interfaces;
using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Data;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Carts.Mappers;

namespace OnlineShoppingPlatform.Carts.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ShoppingDbContext _context;
        private readonly ILogger<CartRepository> _logger;
        private readonly IEncryptionHelper _encryptionHelper;
        public CartRepository(ShoppingDbContext context, ILogger<CartRepository> logger, IEncryptionHelper encryptionHelper)
        {
            _context = context;
            _logger = logger;
            _encryptionHelper = encryptionHelper;
        }

        public async Task AddAsync(CartDto cart)
        {
            if (cart == null)
            {
                _logger.LogWarning("Null cart provided for addition.");
                throw new ArgumentNullException(nameof(cart));
            }

            try
            {
                var cartTb = cart.ToEntity(_encryptionHelper);
                await _context.Carts.AddAsync(cartTb);
                await _context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new cart.");
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int cartId)
        {
            try
            {
                var cartEntity = _context.Carts.FirstOrDefault(x => x.CartId == cartId);
                if (cartEntity == null)
                {
                    _logger.LogWarning("Wrong ID provided for deletion");
                    throw new ArgumentOutOfRangeException(cartId.ToString());
                }
                _context.Carts.Remove(cartEntity);
                var result = await _context.SaveChangesAsync();

                return result > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the cart.");
                throw;
            }
        }

        public async Task<CartDto> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                _logger.LogWarning("Null or empty user ID provided for fetching cart.");
                return null!;
            }

            try
            {
                var cartEntity = await _context.Carts
                    .AsNoTracking()
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.UserId == userId);

                if (cartEntity == null)
                {
                    _logger.LogInformation("Cart not found for user ID: {UserId}", userId);
                    return null!;
                }

                var cartDto = cartEntity.ToDto(_encryptionHelper);
                return cartDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching cart for user ID: {UserId}", userId);
                throw;
            }
        }

        public async Task<Cart?> GetCartWithItemsAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);
        }

        public async Task UpdateAsync(Cart cart)
        {
            if (cart == null)
            {
                _logger.LogWarning("Null cart provided for update.");
                throw new ArgumentNullException(nameof(cart));
            }

            try
            {
                _context.Carts.Update(cart);
                await _context.SaveChangesAsync();
                _logger.LogInformation("Successfully updated cart with ID: {CartId}", cart.CartId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating cart with ID: {CartId}", cart.CartId);
                throw;
            }
        }
    }
}
