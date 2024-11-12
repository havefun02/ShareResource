using CRUDFramework;
using CRUDFramework.Interfaces;
using ShareResource.Models.Entities;
using ShareResource.Interfaces;
using Microsoft.EntityFrameworkCore;
using ShareResource.Database;

namespace JwtCookiesScheme.Services
{
    public class TokenService : ITokenService<Token>
    {
        private readonly IRepository<Token, AppDbContext> _tokenRepo;
        private readonly IJwtService<User> _jwtService;

        public TokenService(IRepository<Token, AppDbContext> tokenRepo, IJwtService<User> jwtService)
        {
            _tokenRepo = tokenRepo;
            _jwtService = jwtService;
        }

        public async Task<Token> GetTokenInfo(string token)
        {
            try
            {
                var dbContext = _tokenRepo.GetDbSet();
                var tokenResult = await dbContext.Include(t => t.User).SingleOrDefaultAsync(t => t.RefreshToken == token);
                if (tokenResult == null) throw new ArgumentException("Can not find token");
                return tokenResult;
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<Token> UpdateTokenAsync(Token token)
        {
            try
            {
                var newResetToken = _jwtService.GenerateRefreshToken();
                token.RefreshToken = newResetToken.token;
                token.ExpiredAt = newResetToken.expiredAt;
                var updatedToken = await _tokenRepo.Update(token);
                return updatedToken;
            }
            catch (Exception ex)
            {
                throw new Exception("Fail to update token", ex);
            }
        }

    }
}