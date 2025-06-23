using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Repositories.Interfaces;
using OnlineShoppingPlatform.Services.Interfaces;

namespace OnlineShoppingPlatform.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        public ProductService(IProductRepository productRepository)
        {
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _productRepository.GetAllAsync();
        }

        public async Task<Product> GetByIdAsync(int productId)
        {
            return await _productRepository.GetByIdAsync(productId);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            await _productRepository.AddAsync(product);
            await _productRepository.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(int productId, Product updatedProduct)
        {
            var existing = await _productRepository.GetByIdAsync(productId);
            if (existing == null) return null;

            // Map the changes (or use an external mapper like AutoMapper)
            existing.Name = updatedProduct.Name;
            existing.Description = updatedProduct.Description;
            existing.Price = updatedProduct.Price;
            existing.StockQuantity = updatedProduct.StockQuantity;
            existing.IsInStock = updatedProduct.IsInStock;

            _productRepository.Update(existing);
            await _productRepository.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(int productId)
        {
            var product = await _productRepository.GetByIdAsync(productId);
            if (product == null) return false;

            _productRepository.Delete(product);
            return await _productRepository.SaveChangesAsync();
        }
    }
}
