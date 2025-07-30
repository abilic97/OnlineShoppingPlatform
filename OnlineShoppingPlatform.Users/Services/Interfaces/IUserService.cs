using OnlineShoppingPlatform.Infrastructure.Entities;
using System.Security.Claims;

namespace OnlineShoppingPlatform.Users.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> ProcessExternalLoginAsync(ClaimsPrincipal principal, string? scheme);
        Task<User> CreateLocalUserAsync(string externalUserId, string email, string provider);
        string GenerateJwtToken(User user);
    }
}
