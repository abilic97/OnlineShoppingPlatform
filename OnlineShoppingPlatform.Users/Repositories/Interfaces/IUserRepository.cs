using OnlineShoppingPlatform.Infrastructure.Entities;

namespace OnlineShoppingPlatform.Users.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<User?> GetUserByOAuthAsync(string provider, string externalUserId);
        Task<UserRole?> GetRoleByNameAsync(string roleName);
        Task AddUserRoleAsync(UserRole role);
        Task AddUserAsync(User user);
    }
}
