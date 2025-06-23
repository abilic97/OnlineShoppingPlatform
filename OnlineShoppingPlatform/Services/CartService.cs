using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Repositories.Interfaces;
using OnlineShoppingPlatform.Services.Interfaces;

namespace OnlineShoppingPlatform.Services
{
    public class CartService : ICartService
    {
        private readonly ICartRepository _cartRepository;
        public CartService(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<IEnumerable<Cart>> GetAllAsync()
        {
            return await _cartRepository.GetAllAsync();
        }

        public async Task<Cart> GetByIdAsync(int cartId)
        {
            return await _cartRepository.GetByIdAsync(cartId);
        }

        public async Task<Cart> CreateAsync(Cart cart)
        {
            await _cartRepository.AddAsync(cart);
            await _cartRepository.SaveChangesAsync();
            return cart;
        }

        public async Task<Cart> UpdateStatusAsync(int cartId, string newStatus)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return null;

            cart.Status = newStatus;
            _cartRepository.Update(cart);
            await _cartRepository.SaveChangesAsync();
            return cart;
        }

        public async Task<bool> DeleteAsync(int cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return false;

            _cartRepository.Delete(cart);
            return await _cartRepository.SaveChangesAsync();
        }

        // Example: Recalculate totals (simple version)
        public async Task<Cart> RecalculateTotalsAsync(int cartId)
        {
            var cart = await _cartRepository.GetByIdAsync(cartId);
            if (cart == null) return null;

            decimal subtotal = 0;
            if (cart.Items != null)
            {
                foreach (var item in cart.Items)
                {
                    if (item?.Product != null)
                    {
                        subtotal += item.Quantity * item.Product.Price;
                    }
                }
            }

            cart.Subtotal = subtotal;
            // Suppose shipping cost is fixed or based on logic
            // cart.ShippingCost = ...
            cart.Total = cart.Subtotal + cart.ShippingCost;

            _cartRepository.Update(cart);
            await _cartRepository.SaveChangesAsync();
            return cart;
        }
    }
}
