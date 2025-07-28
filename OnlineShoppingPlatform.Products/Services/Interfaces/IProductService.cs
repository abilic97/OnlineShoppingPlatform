using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Products.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int productId);
    }
}
