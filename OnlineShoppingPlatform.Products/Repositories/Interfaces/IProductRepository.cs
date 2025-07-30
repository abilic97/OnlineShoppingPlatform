using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Products.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int productId);
        Task<List<Product>> GetByIdsAsync(IEnumerable<int> productIds);
    }
}
