using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineShoppingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class HealthcheckController : ControllerBase
    {
        public HealthcheckController()
        {

        }
    }
}
