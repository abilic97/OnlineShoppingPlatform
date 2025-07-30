using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Carts.Mappers;
using OnlineShoppingPlatform.Carts.Repositories.Interfaces;
using OnlineShoppingPlatform.Carts.Services.Interfaces;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Infrastructure.Exceptions;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Carts.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CartService> _logger;
        private readonly IEncryptionHelper _encryptionHelper;

        private const decimal ShippingCost = 25.00m;

        public CartService(ICartRepository cartRepository,
            IProductRepository productRepository,
            IEncryptionHelper encryptionHelper,
            ILogger<CartService> logger)
        {
            _cartRepository = cartRepository;
            _productRepository = productRepository;
            _encryptionHelper = encryptionHelper;
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
                var result = await _cartRepository.DeleteAsync(cartId);

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
                var cart = await GetValidatedCartAsync(cartId);
                var product = await GetValidatedProductAsync(newItem.ProductId);

                AddOrUpdateCartItem(cart, product, newItem.Quantity);

                RecalculateCartTotals(cart);
                await _cartRepository.UpdateAsync(cart);

                _logger.LogInformation("Added item to cart {CartId}", cartId);
                return cart.ToDto(_encryptionHelper);
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
                var cart = await GetValidatedCartAsync(cartId);

                var item = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
                if (item == null)
                {
                    _logger.LogWarning("Cart item not found with ID: {CartItemId} in cart {CartId}", cartItemId, cartId);
                    throw new ArgumentOutOfRangeException(nameof(cartItemId), "Cart item not found.");
                }

                cart.Items.Remove(item);

                RecalculateCartTotals(cart);
                await _cartRepository.UpdateAsync(cart);

                _logger.LogInformation("Removed item {CartItemId} from cart {CartId}", cartItemId, cartId);
                return cart.ToDto(_encryptionHelper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item {CartItemId} from cart {CartId}", cartItemId, cartId);
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

        private void RecalculateCartTotals(Cart cart)
        {
            cart.Subtotal = cart.Items.Sum(i => i.Quantity * i.Product.Price);
            cart.ShippingCost = ShippingCost;
            cart.Total = cart.Subtotal + cart.ShippingCost;
        }

        private async Task<Cart> GetValidatedCartAsync(int cartId)
        {
            var cart = await _cartRepository.GetCartWithItemsAsync(cartId);
            if (cart == null)
            {
                _logger.LogWarning("Cart not found with ID: {CartId}", cartId);
                throw new CartNotFoundException(cartId);
            }

            return cart;
        }

        private async Task<Product> GetValidatedProductAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null)
            {
                _logger.LogWarning("Product not found with ID: {ProductId}", productId);
                throw new EntityNotFoundException("Product", productId);
            }

            return product;
        }

        private void AddOrUpdateCartItem(Cart cart, Product product, int quantity)
        {
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                cart.Items.Add(new CartItem
                {
                    ProductId = product.ProductId,
                    Quantity = quantity,
                    Product = product,
                    CartId = cart.CartId,
                    CreatedOn = DateTime.UtcNow,
                    CreatedBy = "System"
                });
            }
        }
    }
}