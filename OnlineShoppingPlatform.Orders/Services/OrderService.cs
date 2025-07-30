using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Orders.Factories.Interfaces;
using OnlineShoppingPlatform.Orders.Repositories.Interfaces;
using OnlineShoppingPlatform.Orders.Services.Interfaces;
using OnlineShoppingPlatform.Products.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Orders.Services
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderFactory _orderFactory;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<OrderService> _logger;

        public OrderService(IOrderRepository orderRepository,
            IOrderFactory orderFactory,
            IProductRepository productRepository,
            ILogger<OrderService> logger)
        {
            _orderRepository = orderRepository;
            _orderFactory = orderFactory;
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<int> PlaceOrderAsync(CartDto cartDto)
        {
            _logger.LogInformation("Starting to place order for user {UserId}", cartDto.UserId);
            ValidateCart(cartDto);

            var productIds = cartDto.Items.Select(i => i.ProductId).Distinct();
            var products = await _productRepository.GetByIdsAsync(productIds);
            var productDict = products.ToDictionary(p => p.ProductId);

            var order = _orderFactory.CreateOrder(cartDto, productDict);

            await _orderRepository.AddAsync(order);
            return order.OrderId;
        }

        private void ValidateCart(CartDto cartDto)
        {
            if (cartDto == null)
                throw new ArgumentNullException(nameof(cartDto), "Cart cannot be null.");

            if (cartDto.Items == null || !cartDto.Items.Any())
                throw new ArgumentException("Cart cannot be empty.", nameof(cartDto.Items));

            if (cartDto.Total <= 0)
                throw new ArgumentException("Total amount must be greater than zero.", nameof(cartDto.Total));
        }
    }
}
