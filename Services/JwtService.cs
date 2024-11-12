using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ShareResource.Interfaces;
using ShareResource.Models.Entities;
using ShareResource.Interfaces;
using ShareResource.Models.Entities;

namespace ShareResource.Services
{
    public class JwtService : IJwtService<User>
    {
        private readonly string _secretKey;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _tokenExpiration;
        private readonly int _refreshExpiration;

        public JwtService(IConfiguration configuration)
        {
            var jwtVariables = configuration.GetSection("JWT");
            if (jwtVariables != null)
            {
                _secretKey = jwtVariables["Key"] ?? throw new ArgumentNullException("Key is not set");
                _issuer = jwtVariables["Issuer"] ?? throw new ArgumentNullException("Issuer is not set");
                _audience = jwtVariables["Audience"] ?? throw new ArgumentNullException("Audience is not set");
                _tokenExpiration = jwtVariables.GetValue<int>("AccessTokenExpiresInMinutes");
                _refreshExpiration = jwtVariables.GetValue<int>("RefreshTokenExpiresInDays");
            }
            else
            {
                throw new ArgumentNullException("Cannot read JWT config");
            }
        }

        public string GenerateAccessToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Role, user.UserRoleId),
                new Claim(ClaimTypes.Name, user.UserName)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_tokenExpiration),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateAccessToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidIssuer = _issuer,
                    ValidAudience = _audience,
                    ClockSkew = TimeSpan.Zero // Reduce delay when validating token
                }, out SecurityToken validatedToken);

                return principal; // Return the validated ClaimsPrincipal
            }
            catch (Exception)
            {
                throw;
            }
        }

        public RefreshTokenResult GenerateRefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            string token = Convert.ToBase64String(randomBytes);
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(_refreshExpiration);
            return new RefreshTokenResult { token = token, expiredAt = refreshTokenExpiration };
        }
    }
}