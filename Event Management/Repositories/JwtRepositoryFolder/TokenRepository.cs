using Event_Management.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Event_Management.Repositories.JwtRepositoryFolder
{
    public class TokenRepository : ITokenRepository
    {
        private readonly IConfiguration _configuration; // Configuration object to access app settings

        public TokenRepository(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        /// <summary>
        /// Generates a JWT token for the user.
        public string GenerateToken(User user)
        {
            // Create a new instance of JwtSecurityTokenHandler
            var securityHandler = new JwtSecurityTokenHandler();

            // Get the key from the configuration
            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]);

            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Name),
                    new Claim(ClaimTypes.Uri, user.ProfilePicture),
                    new Claim(ClaimTypes.Hash, user.PasswordHash),
                    new Claim(ClaimTypes.Role, user.Role.ToString().ToUpper()),
                    new Claim("user_type", user.UserType.ToString()),
                    new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
                Audience = _configuration["Jwt:Audience"],
                Issuer = _configuration["Jwt:Issuer"]
            };

            // Create the token using the security handler
            SecurityToken securityToken = securityHandler.CreateToken(tokenDescriptor);

            // Write the token to a string
            string token = securityHandler.WriteToken(securityToken);

            return token;
        }
    }
}
