using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OnlineShoppingPlatform.Data;
using OnlineShoppingPlatform.Data.Entities;
using OnlineShoppingPlatform.Services.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShoppingPlatform.Services
{
    public class UserService : IUserService
    {
        private readonly ShoppingDbContext _context;
        private readonly IConfiguration _configuration;
        public UserService(ShoppingDbContext shoppingDbContext, IConfiguration configuration)
        {
            _context = shoppingDbContext;
            _configuration = configuration;
        }

        public async Task<User> CreateLocalUserAsync(string externalUserId, string email, string provider)
        {
            var user = await _context.Users
                .Include(u => u.UserRole)
                .SingleOrDefaultAsync(u => u.OAuthProvider == provider && u.OAuthId == externalUserId);

            if (user == null)
            {
                var customerRole = await _context.UserRoles
                    .FirstOrDefaultAsync(r => r.RoleName == "Customer");

                if (customerRole == null)
                {
                    customerRole = new UserRole
                    {
                        RoleName = "Customer",
                        CreatedBy = "System",
                        UpdatedBy = "System",
                        CreatedOn = DateTime.UtcNow,
                        UpdatedOn = DateTime.UtcNow
                    };
                    _context.UserRoles.Add(customerRole);
                    await _context.SaveChangesAsync();
                }

                user = new User
                {
                    Username = email,
                    OAuthId = externalUserId,
                    OAuthProvider = provider,
                    UserRole = customerRole,
                    CreatedBy = "System",
                    UpdatedBy = "System",
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow
                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
            }

            return user;
        }

        public string GenerateJwtToken(User user)
        {
            var claims = new[]
            {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, user.Username),
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
            new Claim(ClaimTypes.Role, user.UserRole.RoleName)
        };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
