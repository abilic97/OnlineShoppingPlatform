using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShoppingPlatform.Cart.Repositories.Interfaces
{
    public interface ICartRepository
    {
        Task<CartDto> GetByIdAsync(int cartId);
        Task AddAsync(CartDto cart);
        void Update(CartDto cart);
        void Delete(CartDto cart);
        Task<bool> SaveChangesAsync();
        Task<CartDto> GetByUserIdAsync(string userId);
        Task<CartDto?> AddItemAsync(int cartId, CartItemDto newItem);
        Task<CartDto?> RemoveItemAsync(int cartId, int cartItemId);
    }
}
