using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Data;
using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Repositories
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
                _logger.LogError(ex, "Error occurred while retrieving all products.");
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            if (productId <= 0)
            {
                _logger.LogWarning("Invalid product ID provided: {ProductId}", productId);
                return null!;
            }

            try
            {
                return await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while retrieving product with ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task AddAsync(Product product)
        {
            if (product == null)
            {
                _logger.LogWarning("Null product provided for addition.");
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                await _context.Products.AddAsync(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while adding a new product.");
                throw;
            }
        }

        public void Update(Product product)
        {
            if (product == null)
            {
                _logger.LogWarning("Null product provided for update.");
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                _context.Products.Update(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating the product.");
                throw;
            }
        }

        public void Delete(Product product)
        {
            if (product == null)
            {
                _logger.LogWarning("Null product provided for deletion.");
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                _context.Products.Remove(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting the product.");
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
    }
}
