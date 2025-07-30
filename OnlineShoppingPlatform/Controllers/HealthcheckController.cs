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
