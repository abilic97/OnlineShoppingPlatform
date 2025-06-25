using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Data;
using OnlineShoppingPlatform.Data.Entities;
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
                return null!;

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

        public async Task<CartDto> GetByUserIdAsync(string userId)
        {
            var cartEntity = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cartEntity == null)
                return null!;

            var cartDto = cartEntity.ToDto();
            return cartDto;
        }

        public async Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem)
        {
            var cart = await _context.Carts
                .Include(c => c.Items)
                .ThenInclude(i => i.Product)
                .FirstOrDefaultAsync(c => c.CartId == cartId);

            if (cart == null)
                return null;

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == newItem.ProductId);

            if (existingItem != null)
            {
                existingItem.Quantity += newItem.Quantity;
            }
            else
            {
                var product = await _context.Products.FindAsync(newItem.ProductId);
                if (product == null)
                    return null;

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

            cart.Subtotal = cart.Items.Sum(i => i.Quantity * i.Product.Price);
            cart.ShippingCost = 5;
            cart.Total = cart.Subtotal + cart.ShippingCost;

            await _context.SaveChangesAsync();

            return cart.ToDto();
        }
    }
}
