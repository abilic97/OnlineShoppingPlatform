using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Cart.Repositories.Interfaces;
using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Data;
using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Cart.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly ShoppingDbContext _context;
        private readonly ILogger<CartRepository> _logger;
        private readonly IEncryptionHelper _encryptionHelper;
        //TODO: placeholder for now. Shipping cost needs to be defined in coordinance
        //with shipping services (if external services were to be used) or defined within
        //the company itself if it also provides shipping services along with selling of goods
        //After that we can have predefined DB value based on the shipping company and/or shipping address
        private const decimal ShippingCost = 25.00m;
        public CartRepository(ShoppingDbContext context, ILogger<CartRepository> logger, IEncryptionHelper encryptionHelper)
        {
            _context = context;
            _logger = logger;
            _encryptionHelper = encryptionHelper;
        }

        public async Task<CartDto> GetByIdAsync(int cartId)
        {
            try
            {
                if (cartId <= 0)
                {
                    _logger.LogWarning("Invalid cart ID provided: {CartId}", cartId);
                    return null!;
                }

                var cartEntity = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cartEntity == null)
                {
                    _logger.LogInformation("Cart not found with ID: {CartId}", cartId);
                    return null!;
                }

                var cartDto = cartEntity.ToDto(_encryptionHelper);
                return cartDto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching cart with ID: {CartId}", cartId);
                throw;
            }
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new cart.");
                throw;
            }
        }

        public void Update(CartDto cart)
        {
            if (cart == null)
            {
                _logger.LogWarning("Null cart provided for update.");
                throw new ArgumentNullException(nameof(cart));
            }

            try
            {
                var cartTb = cart.ToEntity(_encryptionHelper);
                _context.Carts.Update(cartTb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the cart.");
                throw;
            }
        }

        public void Delete(CartDto cart)
        {
            if (cart == null)
            {
                _logger.LogWarning("Null cart provided for deletion.");
                throw new ArgumentNullException(nameof(cart));
            }

            try
            {
                var cartId = int.Parse(_encryptionHelper.Decrypt(cart.CartId));
                var cartTb = _context.Carts.FirstOrDefault(x => x.CartId == cartId);
                if (cartTb == null)
                {
                    _logger.LogWarning("Wrong ID provided for deletion");
                    throw new ArgumentOutOfRangeException(cart.CartId.ToString());
                }
                _context.Carts.Remove(cartTb);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the cart.");
                throw;
            }
        }
        public async Task<bool> SaveChangesAsync()
        {
            try
            {
                return (await _context.SaveChangesAsync()) > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while saving changes to the database.");
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

        public async Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem)
        {
            if (newItem == null)
            {
                _logger.LogWarning("Null item provided for addition to the cart with ID: {CartId}", cartId);
                return null;
            }

            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogInformation("Cart not found with ID: {CartId}", cartId);
                    return null;
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);

                if (existingItem != null)
                {
                    existingItem.Quantity += newItem.Quantity;
                }
                else
                {
                    var product = await _context.Products.FindAsync(newItem.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("Product not found with ID: {ProductId}", newItem.ProductId);
                        return null;
                    }

                    var newCartItem = new CartItem
                    {
                        ProductId = product.ProductId,
                        Quantity = newItem.Quantity,
                        Product = product,
                        CartId = cartId,
                        CreatedBy = "System",
                        CreatedOn = DateTime.UtcNow,
                    };

                    cart.Items.Add(newCartItem);
                }

                RecalculateCartTotals(cart);
                await _context.SaveChangesAsync();

                return cart.ToDto(_encryptionHelper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding item to the cart with ID: {CartId}", cartId);
                throw;
            }
        }

        public async Task<CartDto?> RemoveItemAsync(int cartId, int cartItemId)
        {
            try
            {
                var cart = await _context.Carts
                    .Include(c => c.Items)
                    .ThenInclude(i => i.Product)
                    .FirstOrDefaultAsync(c => c.CartId == cartId);

                if (cart == null)
                {
                    _logger.LogInformation("Cart not found with ID: {CartId}", cartId);
                    return null;
                }

                var existingItem = cart.Items.FirstOrDefault(i => i.CartItemId == cartItemId);
                if (existingItem == null)
                {
                    _logger.LogInformation("Cart item not found with ID: {CartItemId} in cart with ID: {CartId}", cartItemId, cartId);
                    return null;
                }

                cart.Items.Remove(existingItem);

                RecalculateCartTotals(cart);
                await _context.SaveChangesAsync();

                return cart.ToDto(_encryptionHelper);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while removing item from the cart with ID: {CartId}", cartId);
                throw;
            }
        }

        private void RecalculateCartTotals(Cart cart)
        {
            cart.Subtotal = cart.Items.Sum(i => i.Quantity * i.Product.Price);
            cart.ShippingCost = ShippingCost;
            cart.Total = cart.Subtotal + cart.ShippingCost;
        }
    }
