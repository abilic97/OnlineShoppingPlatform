using OnlineShoppingPlatform.Data.Entities;

namespace OnlineShoppingPlatform.Services.Interfaces
{
    public interface IUserService
    {
        Task<User> CreateLocalUserAsync(string externalUserId, string email, string provider);
        string GenerateJwtToken(User user);
    }
}
