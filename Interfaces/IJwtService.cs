using System.Security.Claims;

namespace ShareResource.Interfaces
{
    public class RefreshTokenResult
    {
        public string? token {  get; set; }
        public DateTime expiredAt { get; set; }
    }
    public interface IJwtService<T>
    {
        string GenerateToken(T user);
        ClaimsPrincipal ValidateToken(string token);
        RefreshTokenResult RefreshToken();


    }
}
