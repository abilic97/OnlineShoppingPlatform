using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Repositories.Interfaces;
using OnlineShoppingPlatform.Services.Interfaces;

namespace OnlineShoppingPlatform.Services
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

        public async Task<Product> CreateAsync(Product product)
        {
            if (product == null)
            {
                _logger.LogWarning("Null product provided for creation.");
                throw new ArgumentNullException(nameof(product));
            }

            try
            {
                await _productRepository.AddAsync(product);
                await _productRepository.SaveChangesAsync();
                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while creating a product.");
                throw;
            }
        }

        public async Task<Product> UpdateAsync(int productId, Product updatedProduct)
        {
            if (updatedProduct == null)
            {
                _logger.LogWarning("Null product provided for update.");
                throw new ArgumentNullException(nameof(updatedProduct));
            }

            try
            {
                var existing = await _productRepository.GetByIdAsync(productId);
                if (existing == null)
                {
                    _logger.LogInformation("Product not found with ID: {ProductId}", productId);
                    return null;
                }

                existing.Name = updatedProduct.Name;
                existing.Description = updatedProduct.Description;
                existing.Price = updatedProduct.Price;
                existing.StockQuantity = updatedProduct.StockQuantity;
                existing.IsInStock = updatedProduct.IsInStock;

                _productRepository.Update(existing);
                await _productRepository.SaveChangesAsync();
                return existing;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating product with ID: {ProductId}", productId);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            if (productId <= 0)
            {
                _logger.LogWarning("Invalid product ID provided for deletion: {ProductId}", productId);
                return false;
            }

            try
            {
                var product = await _productRepository.GetByIdAsync(productId);
                if (product == null)
                {
                    _logger.LogInformation("Product not found for deletion with ID: {ProductId}", productId);
                    return false;
                }

                _productRepository.Delete(product);
                return await _productRepository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while deleting product with ID: {ProductId}", productId);
                throw;
            }
        }
    }
}
