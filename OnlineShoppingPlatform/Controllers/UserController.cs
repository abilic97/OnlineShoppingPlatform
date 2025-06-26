using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.Services.Interfaces;
using System.Security.Claims;

namespace OnlineShoppingPlatform.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("login/{provider}")]
        public IActionResult Login(string provider)
        {
            string redirectUrl = Url.Action("OAuthCallback", "User");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
                return BadRequest("External authentication error");

            var externalUserId = authenticateResult.Principal.FindFirstValue(ClaimTypes.NameIdentifier);
            var email = authenticateResult.Principal.FindFirstValue(ClaimTypes.Email);

            // Fallback: Try getting the provider from the current user identity (e.g., via Claim or AuthenticationType)
            var provider = authenticateResult.Ticket?.AuthenticationScheme
                           ?? authenticateResult.Principal.Identity?.AuthenticationType
                           ?? "Unknown";

            var user = await _userService.CreateLocalUserAsync(externalUserId, email, provider);

            var token = _userService.GenerateJwtToken(user);

            var redirectUrl = $"http://localhost:25507/auth-callback?token={token}";
            return Redirect(redirectUrl);
        }
    }
}
