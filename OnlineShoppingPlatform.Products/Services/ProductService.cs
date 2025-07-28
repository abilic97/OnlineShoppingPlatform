using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;
using OnlineShoppingPlatform.Products.Services.Interfaces;

namespace OnlineShoppingPlatform.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        public ProductService(IProductRepository productRepository, ILogger<ProductService> logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            try
            {
                return await _productRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while getting all products.");
                throw;
            }
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            if (productId <= default(int))
            {
                _logger.LogWarning("Invalid product ID provided: {ProductId}", productId);
                return null!;
            }

            try
            {
                return await _productRepository.GetByIdAsync(productId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product with ID: {ProductId}", productId);
                throw;
            }
        }
    }
}
