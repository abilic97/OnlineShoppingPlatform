using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Users.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateLocalUserAsync(string externalUserId, string email, string provider);
        string GenerateJwtToken(User user);
    }
}
