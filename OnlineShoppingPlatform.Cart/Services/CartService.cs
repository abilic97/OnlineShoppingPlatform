using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Carts.Repositories.Interfaces;
using OnlineShoppingPlatform.Carts.Services.Interfaces;
using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Exceptions;

namespace OnlineShoppingPlatform.Carts.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(ICartRepository cartRepository,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<CartDto> GetOrCreateUserCartAsync(string userId)
        {
            var cart = await _cartRepository.GetByUserIdAsync(userId);
            return cart ?? await CreateAsync(CreateNewCart(userId));
        }

        public async Task<bool> DeleteAsync(int cartId)
        {
            try
            {
                var cart = await _cartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    _logger.LogWarning("Delete failed. Cart {CartId} not found.", cartId);
                    return false;
                }

                _cartRepository.Delete(cart);
                var result = await _cartRepository.SaveChangesAsync();

                _logger.LogInformation("Deleted cart {CartId}", cartId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart {CartId}", cartId);
                throw;
            }
        }

        public async Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem)
        {
            if (newItem == null) throw new ArgumentNullException(nameof(newItem));

            try
            {
                var cart = await _cartRepository.AddItemAsync(cartId, newItem);
                if (cart == null)
                {
                    _logger.LogWarning("Add item failed. Cart {CartId} not found.", cartId);
                    throw new CartNotFoundException(cartId);
                }
                else
                {
                    _logger.LogInformation("Added item to cart {CartId}", cartId);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding item to cart {CartId}", cartId);
                throw;
            }
        }

        public async Task<CartDto?> RemoveItemAsync(int cartId, int cartItemId)
        {
            try
            {
                var cart = await _cartRepository.RemoveItemAsync(cartId, cartItemId);
                if (cart == null)
                {
                    _logger.LogWarning("Remove item failed. Cart {CartId} or item {CartItemId} not found.", cartId, cartItemId);
                    throw new CartNotFoundException(cartId);
                }
                else
                {
                    _logger.LogInformation("Removed item {CartItemId} from cart {CartId}", cartItemId, cartId);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error removing item {CartItemId} from cart {CartId}", cartItemId, cartId);
                throw;
            }
        }

        private async Task<CartDto> CreateAsync(CartDto cart)
        {
            if (cart == null) throw new ArgumentNullException(nameof(cart));
            try
            {
                _logger.LogInformation("Creating new cart for user {UserId}", cart.UserId);
                await _cartRepository.AddAsync(cart);
                await _cartRepository.SaveChangesAsync();
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating cart for user {UserId}", cart.UserId);
                throw;
            }
        }

        private CartDto CreateNewCart(string userId)
        {
            return new CartDto
            {
                UserId = userId,
                CartNumber = Guid.NewGuid().ToString(),
                ExpiresAt = DateTime.UtcNow.AddMinutes(30)
            };
        }
    }
}