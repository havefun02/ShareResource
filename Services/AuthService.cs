using ShareResource.Interfaces;
using CRUDFramework.Cores;
using CRUDFramework.Interfaces;
using ShareResource.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ShareResource.Models.Dtos;
using Microsoft.AspNetCore.Identity;
using ShareResource.Database;

namespace ShareResource.Services
{
    public class AuthService : IAuthService<User, Token>
    {
        private readonly IRepository<User,AppDbContext> _userRepository;
        private readonly IRepository<Token, AppDbContext> _tokenRepository;
        private readonly IJwtService<User> _jwtService;

        public AuthService(IRepository<User, AppDbContext> userRepository, IRepository<Token, AppDbContext> tokenRepository, IJwtService<User> jwtService)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _jwtService = jwtService;
        }

        public async Task<Token> GetTokenInfoAsync(string token)
        {
            var dbSet = _tokenRepository.GetDbSet();
            try
            {
                var tokenResult = await dbSet.Include(t => t.User)
                                              .SingleOrDefaultAsync(t => t.RefreshToken == token);
                if (tokenResult == null) {
                    throw new NullReferenceException("Can not find the token");
                }
                return tokenResult;
            }
            catch { throw; }
        }

        public async Task<Token> UpdateTokenAsync(Token token)
        {
            try
            {
                var tokenResult = _jwtService.RefreshToken();
                token.RefreshToken = tokenResult.token;
                token.ExpiredAt = tokenResult.expiredAt;
                token.IsRevoked = false;
                var updatedToken = await _tokenRepository.Update(token);
                if (updatedToken == null) {
                    throw new NullReferenceException("Can not update token");
                }
                return updatedToken;
            }
            catch {
                throw;
            }
        }

        

        public async Task<(string, string)> Login(LoginDto dto)
        {
            var userContext=_userRepository.GetDbSet();
            var tokenContext = _tokenRepository.GetDbSet();


            var user = await userContext.SingleOrDefaultAsync(u=>u.UserEmail==dto.Email); // Assuming this method exists

            if (user == null)
            {
                throw new ArgumentException("Invalid email or password.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.UserPassword!, dto.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new ArgumentException("Invalid email or password.");
            }

            var accessToken = _jwtService.GenerateToken(user);
            var refreshToken = _jwtService.RefreshToken();
            try
            {
                var userToken = await tokenContext.SingleOrDefaultAsync(t => t.UserId == user.UserId);
                if (userToken != null)
                {
                    userToken.RefreshToken = refreshToken.token!;
                    userToken.ExpiredAt = refreshToken.expiredAt;
                    await _tokenRepository.Update(userToken);
                }
                else
                {
                    await _tokenRepository.CreateAsync(new Token { RefreshToken=refreshToken.token,ExpiredAt=refreshToken.expiredAt,UserId=user.UserId});
                }
            }
            catch (Exception ex) {
                throw new Exception("Internal server error during query user login operation "+ ex.Message);
            }
            return (accessToken, refreshToken.token!);
        }

        public async Task<bool> Logout(string userId)
        {
            try
            {
                var tokenContext = _tokenRepository.GetDbSet();
                var token = await tokenContext.SingleOrDefaultAsync(t => t.UserId == userId);
                if (token == null)
                {
                    throw new NullReferenceException("Invalid user");
                }
                token.IsRevoked = true;
                var updateResult = await _tokenRepository.Update(token);
                if (updateResult != null) return true;
                else throw new InvalidOperationException("Internal server error during logout");
            }
            catch
            {
                throw;
            }
        }

        public async Task<User> Register(RegisterDto user)
        {
            try
            {
                var userContext = _userRepository.GetDbSet();
                var existingUser = await userContext.SingleOrDefaultAsync(u => u.UserEmail == user.Email);
                if (existingUser != null)
                {
                    throw new ArgumentException("User with this email already exists.");
                }

                var newUser = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserName = user.UserName,
                    UserEmail = user.Email,
                    UserPhone = user.UserPhone,
                    UserRoleId = "Guest"
                };

                var passwordHasher = new PasswordHasher<User>();
                newUser.UserPassword = passwordHasher.HashPassword(newUser, user.Password);

                var userCreated = await _userRepository.CreateAsync(newUser);
                if (userCreated == null) throw new InvalidOperationException("Cant not create user");
                var getDataResult = await userContext.Include(u => u.UserRole).ThenInclude(r => r!.RolePermissions).SingleOrDefaultAsync(u => u.UserId == userCreated.UserId);
                if (getDataResult == null) throw new InvalidOperationException("Internal error finding user data");
                return getDataResult;
            }
            catch { throw; }
        }

        public async Task<bool> UpdatePassword(UpdatePasswordDto dto,string userId)
        {
            var user = await _userRepository.FindOneById(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var passwordHasher = new PasswordHasher<User>();
            user.UserPassword = passwordHasher.HashPassword(user, dto.NewPassword);
            var updateResult=await _userRepository.Update(user);
            if (updateResult == null) throw new InvalidOperationException("Internal error getting user data");
            return true;
        }
        public async Task<bool> ChangePassword(ChangePasswordDto dto)
        {
            try
            {
                var userContext = _userRepository.GetDbSet();
                var user = await userContext.SingleOrDefaultAsync(u => u.UserEmail == dto.UserEmail);
                if (user == null)
                {
                    throw new ArgumentException("User not found.");
                }

                var passwordHasher = new PasswordHasher<User>();
                var result = passwordHasher.VerifyHashedPassword(user, user.UserPassword!, dto.CurrentPassword);

                if (result == PasswordVerificationResult.Failed)
                {
                    throw new ArgumentException("Current password is incorrect.");
                }

                user.UserPassword = passwordHasher.HashPassword(user, dto.NewPassword);
                var updateResult=await _userRepository.Update(user);
                if (updateResult == null) throw new InvalidOperationException("Internal error while changing password");
                return true;
            }
            catch { throw; }
        }

    }
}
