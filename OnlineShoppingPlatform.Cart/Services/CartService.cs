using OnlineShoppingPlatform.Cart.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Cart.Services
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

        public async Task<CartDto> CreateAsync(CartDto cart)
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

        public async Task<CartDto> GetByUserIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId));

            try
            {
                _logger.LogInformation("Fetching cart for user {UserId}", userId);
                var cart = await _cartRepository.GetByUserIdAsync(userId);
                if (cart == null)
                {
                    _logger.LogWarning("Cart for user {UserId} not found", userId);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart for user {UserId}", userId);
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

        private decimal CalculateShippingCost(CartDto cart)
        {
            return 5.00m;
        }
    }
