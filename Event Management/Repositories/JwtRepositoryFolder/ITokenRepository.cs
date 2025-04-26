using Event_Management.Models;

namespace Event_Management.Repositories.JwtRepositoryFolder
{
    public interface ITokenRepository
    {
        string GenerateToken(User user); // Generates a JWT token for the user
    }
}
