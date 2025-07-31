using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Infrastructure.Caching;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;
using OnlineShoppingPlatform.Products.Services.Interfaces;
using System.Text.Json;

namespace OnlineShoppingPlatform.Products.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<ProductService> _logger;
        private readonly IDistributedCache _cache;
        public ProductService(IProductRepository productRepository,
            ILogger<ProductService> logger,
            IDistributedCache cache)
        {
            _productRepository = productRepository;
            _logger = logger;
            _cache = cache;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            const string cacheKey = CacheKeys.AllProducts;
            try
            {
                var cached = await _cache.GetStringAsync(cacheKey);
                if (!string.IsNullOrEmpty(cached))
                {
                    var deserializedProducts = JsonSerializer.Deserialize<List<Product>>(cached);
                    if (deserializedProducts != null)
                    {
                        _logger.LogInformation("Products loaded from cache.");
                        return deserializedProducts;
                    }

                    _logger.LogWarning("Deserialization returned null for cached products.");
                }

                var products = await _productRepository.GetAllAsync();

                var serialized = JsonSerializer.Serialize(products);
                await _cache.SetStringAsync(cacheKey, serialized, new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(10)
                });

                _logger.LogInformation("Products loaded from DB and cached.");
                return products;
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
                throw new ArgumentOutOfRangeException(nameof(productId), "Product ID out of bounds.");
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
