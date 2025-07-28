using Microsoft.EntityFrameworkCore;
using OnlineShoppingPlatform.Infrastructure.Data;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Users.Repositories.Interfaces;

namespace OnlineShoppingPlatform.Users.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ShoppingDbContext _context;

        public UserRepository(ShoppingDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetUserByOAuthAsync(string provider, string externalUserId)
        {
            return await _context.Users
                .Include(u => u.UserRole)
                .SingleOrDefaultAsync(u => u.OAuthProvider == provider && u.OAuthId == externalUserId);
        }

        public async Task<UserRole?> GetRoleByNameAsync(string roleName)
        {
            return await _context.UserRoles.FirstOrDefaultAsync(r => r.RoleName == roleName);
        }

        public async Task AddUserRoleAsync(UserRole role)
        {
            _context.UserRoles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
