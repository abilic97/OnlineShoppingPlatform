using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OnlineShoppingPlatform.Infrastructure.Data;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Orders.Repositories.Interfaces;
namespace OnlineShoppingPlatform.Orders.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ShoppingDbContext _context;
        private readonly ILogger<OrderRepository> _logger;
        public OrderRepository(ShoppingDbContext context, ILogger<OrderRepository> logger)
        {
            _context = context;
            _logger = logger;
        }
        public async Task AddAsync(Order order)
        {
            try
            {
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                _logger.LogInformation("Order {OrderId} added successfully for user {UserId}.", order.OrderId, order.UserId);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "A database error occurred while adding order for user {UserId}.", order.UserId);
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred while adding order for user {UserId}.", order.UserId);
                throw;
            }
        }
    }
}
