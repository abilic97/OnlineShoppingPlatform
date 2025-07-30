using OnlineShoppingPlatform.Carts.DTO;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Infrastructure.Exceptions;
using OnlineShoppingPlatform.Orders.Factories.Interfaces;

namespace OnlineShoppingPlatform.Orders.Factories
{
    public class OrderFactory : IOrderFactory
    {
        private readonly IEncryptionHelper _encryptionHelper;
        private const int OrderNumberLength = 8;

        public OrderFactory(IEncryptionHelper encryptionHelper)
        {
            _encryptionHelper = encryptionHelper;
        }

        public Order CreateOrder(CartDto cartDto, Dictionary<int, Product> productDict)
        {
            if (!_encryptionHelper.TryDecrypt(cartDto.CartId, out var decryptedCartIdStr) ||
               !int.TryParse(decryptedCartIdStr, out var cartId))
            {
                throw new InvalidEncryptedIdException("Invalid or malformed Cart ID.");
            }

            var order = new Order
            {
                UserId = cartDto.UserId,
                OrderNumber = GenerateOrderNumber(),
                TotalAmount = cartDto.Total,
                CartId = cartId,
                Notes = cartDto.Notes,
                Status = "Pending",
                OrderedAt = DateTime.UtcNow
            };

            foreach (var item in cartDto.Items)
            {
                if (!productDict.TryGetValue(item.ProductId, out var product))
                    throw new EntityNotFoundException("Product", item.ProductId);

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

            return order;
        }

        private string GenerateOrderNumber()
        {
            return Guid.NewGuid().ToString("N")[..OrderNumberLength];
        }
    }
}

