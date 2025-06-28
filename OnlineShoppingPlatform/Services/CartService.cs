using OnlineShoppingPlatform.Domain.DTO;
using OnlineShoppingPlatform.Exceptions;
using OnlineShoppingPlatform.Repositories.Interfaces;
using OnlineShoppingPlatform.Services.Interfaces;

namespace OnlineShoppingPlatform.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CartService> _logger;

        public CartService(ICartRepository cartRepository,
            IProductRepository productRepository,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<CartDto> GetByIdAsync(int cartId)
        {
            try
            {
                _logger.LogInformation("Fetching cart by id {CartId}", cartId);
                var cart = await _cartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    _logger.LogWarning("Cart with id {CartId} not found", cartId);
                    throw new CartNotFoundException(cartId);
                }
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching cart by id {CartId}", cartId);
                throw;
            }
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

        public async Task<CartDto> UpdateStatusAsync(int cartId, string newStatus)
        {
            if (string.IsNullOrWhiteSpace(newStatus))
                throw new ArgumentException("New status must be provided.", nameof(newStatus));

            try
            {
                var cart = await _cartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    _logger.LogWarning("Update status failed. Cart {CartId} not found.", cartId);
                    throw new CartNotFoundException(cartId);
                }

                cart.Status = newStatus;
                _cartRepository.Update(cart);
                await _cartRepository.SaveChangesAsync();

                _logger.LogInformation("Updated cart {CartId} status to {Status}", cartId, newStatus);
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating status for cart {CartId}", cartId);
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

        public async Task<CartDto> RecalculateTotalsAsync(int cartId)
        {
            try
            {
                var cart = await _cartRepository.GetByIdAsync(cartId);
                if (cart == null)
                {
                    _logger.LogWarning("Recalculate totals failed. Cart {CartId} not found.", cartId);
                    throw new CartNotFoundException(cartId);
                }

                decimal subtotal = 0;
                if (cart.Items != null)
                {
                    foreach (var item in cart.Items)
                    {
                        if (item.ProductId != default(int))
                        {
                            var product = _productRepository.GetByIdAsync(item.ProductId).GetAwaiter().GetResult();
                            subtotal += item.Quantity * product.Price;
                        }
                    }
                }

                cart.Subtotal = subtotal;

                cart.ShippingCost = CalculateShippingCost(cart);
                cart.Total = cart.Subtotal + cart.ShippingCost;

                _cartRepository.Update(cart);
                await _cartRepository.SaveChangesAsync();

                _logger.LogInformation("Recalculated totals for cart {CartId}", cartId);
                return cart;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating totals for cart {CartId}", cartId);
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
}
