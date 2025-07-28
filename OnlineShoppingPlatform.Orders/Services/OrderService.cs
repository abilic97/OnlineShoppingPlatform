using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Orders.Repositories.Interfaces;
using OnlineShoppingPlatform.Orders.Services.Interfaces;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;
        private readonly IEncryptionHelper _encryptionHelper;
        private const string OrderStatusPending = "Pending";

        public OrderService(IOrderRepository orderRepository,
            IProductRepository productRepository,
            IEncryptionHelper encryptionHelper,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _encryptionHelper = encryptionHelper;
            _logger = logger;
        }
        public async Task<int> PlaceOrderAsync(CartDto cartDto)
        {
            if (cartDto == null) throw new ArgumentNullException(nameof(cartDto));
            if (cartDto.Items == null)
            {
                _logger.LogWarning("Attempt to place order with empty cart for user {UserId}", cartDto.UserId);
                throw new ArgumentException("Cart cannot be empty", nameof(cartDto.Items));
            }

            _logger.LogInformation("Starting to place order for user {UserId}", cartDto.UserId);
            var order = new Order
            {
                UserId = cartDto.UserId,
                OrderNumber = Guid.NewGuid().ToString().Substring(0, 8),
                TotalAmount = cartDto.Total,
                CartId = int.Parse(_encryptionHelper.Decrypt(cartDto.CartId)),
                Notes = cartDto.Notes,
                Status = OrderStatusPending,
                OrderedAt = DateTime.UtcNow
            };

            foreach (var item in cartDto.Items)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId)
                                ?? throw new Exception($"Product {item.ProductId} not found");

                if (!product.IsInStock || product.StockQuantity < item.Quantity)
                    throw new InsufficientStockException($"Insufficient stock for product '{product.Name}'.");

                order.Items.Add(new OrderItem
                {
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    UnitPrice = product.Price
                });

                product.StockQuantity -= item.Quantity;
                product.IsInStock = product.StockQuantity > 0;
            }

            await _orderRepository.AddAsync(order);
            return order.OrderId;
        }
    }
}
