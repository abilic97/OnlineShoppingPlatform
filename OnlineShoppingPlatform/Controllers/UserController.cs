using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using OnlineShoppingPlatform.Users.Services.Interfaces;

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
            string? redirectUrl = Url.Action("OAuthCallback", "User");

            if (redirectUrl == null)
                return BadRequest("Failed to generate redirect URL.");

            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            return Challenge(properties, provider);
        }

        [HttpGet]
        public async Task<IActionResult> OAuthCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync();

            if (!authenticateResult.Succeeded)
                return BadRequest("External authentication error");

            var user = await _userService.ProcessExternalLoginAsync(
                authenticateResult.Principal!, authenticateResult.Ticket?.AuthenticationScheme);

            var token = _userService.GenerateJwtToken(user);

            var redirectUrl = $"http://localhost:25507/auth-callback?token={token}";
            return Redirect(redirectUrl);
        }
    }
}
