using ShareResource.Interfaces;
using System.Security.Claims;
using System.Text;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using ShareResource.Models.Entities;
namespace ShareResource.Services
{
    public class JwtService : IJwtService<User>
    {
        private readonly string? _secretKey;
        private readonly string? _issuer;
        private readonly string? _audience;
        private readonly int _tokenExpiration;
        private readonly int _refreshExpiration;

        public JwtService(IConfiguration configuration)
        {
            var jwtVaribles = configuration.GetSection("JWT");
            if (jwtVaribles != null)
            {
                if (jwtVaribles.GetSection("Key") != null)
                    _secretKey = jwtVaribles.GetSection("Key").Get<string>()!;

                if (jwtVaribles.GetSection("Issuer") != null)
                    _issuer = jwtVaribles.GetSection("Issuer").Get<string>()!;

                if (jwtVaribles.GetSection("Audience") != null)
                    _audience = jwtVaribles.GetSection("Audience").Get<string>()!;
                if (jwtVaribles.GetSection("AccessTokenExpiresInMinutes") != null)
                    _tokenExpiration = jwtVaribles.GetSection("AccessTokenExpiresInMinutes").Get<int>();
                if (jwtVaribles.GetSection("RefreshTokenExpiresInDays") != null)
                    _refreshExpiration = jwtVaribles.GetSection("RefreshTokenExpiresInDays").Get<int>();
            }
            else
            {
                throw new ArgumentNullException("Cannot read Jwt config");
            }
        }

        public string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var claims =
                new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier,user.UserId!),new Claim(ClaimTypes.Role,user.UserRoleId!)
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.Add(TimeSpan.FromMinutes(_tokenExpiration)),
                Issuer = _issuer,
                Audience = _audience,
                SigningCredentials = creds
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_secretKey!);
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _issuer,
                ValidAudience = _audience,
                ClockSkew = TimeSpan.Zero // Reduce delay when validating token
            }, out SecurityToken validatedToken);

            return (ClaimsPrincipal)tokenHandler.ValidateToken(token, new TokenValidationParameters(), out validatedToken);
        }

        public RefreshTokenResult RefreshToken()
        {
            var randomBytes = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomBytes);
            }
            string token = Convert.ToBase64String(randomBytes);
            var refreshTokenExpiration = DateTime.UtcNow.Add(TimeSpan.FromDays(_refreshExpiration));
            return new RefreshTokenResult { token = token, expiredAt = refreshTokenExpiration };
        }
    }
}
