using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Infrastructure.Data;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Products.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly ShoppingDbContext _context;
        private readonly ILogger<ProductRepository> _logger;
        public ProductRepository(ShoppingDbContext context, ILogger<ProductRepository> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _context.Products.AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving all products. Exception message: {Message}", ex.Message);
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            if (productId <= 0)
            {
                _logger.LogError("Invalid product ID provided: {ProductId}", productId);
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID must be greater than zero.");
            }

            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                if (product == null)
                {
                    _logger.LogError("Product not found. ProductID : {ProductId}", productId);
                    throw new KeyNotFoundException($"Product with ID {productId} was not found.");
                }

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID: {ProductId}", productId);
                throw;
            }
        }
    }
}
