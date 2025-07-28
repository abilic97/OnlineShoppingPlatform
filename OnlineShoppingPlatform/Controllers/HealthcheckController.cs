using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.Infrastructure.Data;

namespace OnlineShoppingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class HealthcheckController : ControllerBase
    {
        private readonly ShoppingDbContext _context;
        public HealthcheckController(ShoppingDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // For this PoC and to satisfy the requirements of the task, since we are
        // checking just DB connection, it is ok to be like this in controller.
        // Otherwise, if there were more stuff like RabbitMQ, external services availability checks, etc.
        // it would be good to move it to a seperate service
        // so it is also more easy to test and setup for unit/integration testing.
        [HttpGet("check-database-connection")]
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { status = "Healthy", message = "Database connection is successful." });
                }
                else
                {
                    return StatusCode(503, new { status = "Unhealthy", message = "Database connection failed." });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "Unhealthy", message = "An error occurred while checking the database connection.", error = ex.Message });
            }
        }
    }
}
