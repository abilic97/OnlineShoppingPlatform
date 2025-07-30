using Microsoft.Extensions.Configuration;
using OnlineShoppingPlatform.Infrastructure.Entities;
using OnlineShoppingPlatform.Users.Services.Interfaces;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using OnlineShoppingPlatform.Users.Repositories.Interfaces;
using System.Security.Claims;

namespace OnlineShoppingPlatform.Users.Services
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        //It would be best to have a mapping table in DB were we have RoleID -> RoleName
        //We would automatically assing all roles as Customers since that's what shopping platform is 
        //usually used for. For admins, who could manually add/delete products, it is best to have seperate
        // registration page, or they should be added manually through DB.
        private const string UserRoleName = "Customer";

        public UserService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<User> ProcessExternalLoginAsync(ClaimsPrincipal principal, string? scheme)
        {
            var externalUserId = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var email = principal?.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            var provider = scheme ?? principal.Identity?.AuthenticationType ?? "Unknown";

            if (string.IsNullOrEmpty(externalUserId) || string.IsNullOrEmpty(email))
                throw new InvalidOperationException("Invalid external login data");

            var user = await CreateLocalUserAsync(externalUserId, email, provider);
            return user;
        }

        public async Task<User> CreateLocalUserAsync(string externalUserId, string email, string provider)
        {
            var user = await _userRepository.GetUserByOAuthAsync(provider, externalUserId);

            if (user == null)
            {
                var customerRole = await _userRepository.GetRoleByNameAsync(UserRoleName);
                if (customerRole == null)
                {
                    customerRole = new UserRole { RoleName = UserRoleName };
                    await _userRepository.AddUserRoleAsync(customerRole);
                }

                user = new User
                {
                    Username = email,
                    OAuthId = externalUserId,
                    OAuthProvider = provider,
                    UserRole = customerRole,
                };

                await _userRepository.AddUserAsync(user);
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
