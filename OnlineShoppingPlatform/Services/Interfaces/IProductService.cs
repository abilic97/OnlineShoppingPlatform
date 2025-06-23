using OnlineShoppingPlatform.Data.Entities;

namespace OnlineShoppingPlatform.Services.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int productId);
        Task<Product> CreateAsync(Product product);
        Task<Product> UpdateAsync(int productId, Product product);
        Task<bool> DeleteAsync(int productId);
    }
}
