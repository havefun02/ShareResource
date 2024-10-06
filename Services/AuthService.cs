using ShareResource.Interfaces;
using CRUDFramework.Cores;
using CRUDFramework.Interfaces;
using ShareResource.Models.Entities;
using Microsoft.EntityFrameworkCore;
using ShareResource.Models.Dtos;
using Microsoft.AspNetCore.Identity;

namespace ShareResource.Services
{
    public class AuthService : IAuthService<User, Token>
    {
        private readonly IRepository<User> _userRepository;
        private readonly IRepository<Token> _tokenRepository;
        private readonly IJwtService<User> _jwtService;

        public AuthService(IRepository<User> userRepository, IRepository<Token> tokenRepository, IJwtService<User> jwtService)
        {
            _userRepository = userRepository;
            _tokenRepository = tokenRepository;
            _jwtService = jwtService;
        }

        public async Task<Token> GetTokenInfoAsync(string token)
        {
            var dbSet = _tokenRepository.GetDbSet();

            var tokenResult = await dbSet.Include(t => t.User)
                                          .SingleOrDefaultAsync(t => t.RefreshToken == token);
            if (tokenResult == null)
            {
                throw new ArgumentException("Token not found.");
            }
            return tokenResult;
        }

        public async Task<Token> UpdateTokenAsync(Token token)
        {
            var tokenResult = _jwtService.RefreshToken();
            if (tokenResult == null)
            {
                throw new ArgumentException("Failed to generate a new token.");
            }
            token.RefreshToken = tokenResult.token;
            token.ExpiredAt = tokenResult.expiredAt;
            token.IsRevoked = false;
            await _tokenRepository.Update(token);
            return token;
        }

        Task IAuthService<User, Token>.ChangePassword(ChangePasswordDto dto)
        {
            throw new NotImplementedException();
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
            var result = passwordHasher.VerifyHashedPassword(user, user.Password!, dto.Password);

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
            }
            catch (Exception ex) {
                throw new Exception("Internal server error during query user login operation "+ ex.Message);
            }
            return (accessToken, refreshToken.token!);
        }

        public async Task Logout(string userId)
        {
            var tokenContext = _tokenRepository.GetDbSet();
            var token= await tokenContext.SingleOrDefaultAsync(t=>t.UserId==userId);
            if (token == null)
            {
                throw new ArgumentException("Invalid user data");
            }
            token.IsRevoked = true;
            await _tokenRepository.Update(token);

        }

        public async Task<User> Register(RegisterDto user)
        {
            var userContext = _userRepository.GetDbSet();
            var tokenContext = _tokenRepository.GetDbSet();

            var existingUser = await userContext.SingleOrDefaultAsync(u=>u.UserEmail==user.Email);
            if (existingUser != null)
            {
                throw new ArgumentException("User with this email already exists.");
            }

            var newUser = new User
            {
                UserName = user.UserName,
                UserEmail = user.Email,
                UserPhone = user.UserPhone,
                UserRoleId = "Admin"
            };

            var passwordHasher = new PasswordHasher<User>();
            newUser.Password = passwordHasher.HashPassword(newUser, user.Password);

            var res=await _userRepository.CreateAsync(newUser);
            return res;
        }

        public async Task UpdatePassword(UpdatePasswordDto dto,string userId)
        {
            var user = await _userRepository.FindOneById(userId);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var passwordHasher = new PasswordHasher<User>();
            user.Password = passwordHasher.HashPassword(user, dto.NewPassword);
            await _userRepository.Update(user);
        }
        public async Task ChangePassword(ChangePasswordDto dto)
        {
            var userContext = _userRepository.GetDbSet();
            var user= await userContext.SingleOrDefaultAsync(u=>u.UserEmail == dto.UserEmail);
            if (user == null)
            {
                throw new ArgumentException("User not found.");
            }

            var passwordHasher = new PasswordHasher<User>();
            var result = passwordHasher.VerifyHashedPassword(user, user.Password!, dto.CurrentPassword);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new ArgumentException("Current password is incorrect.");
            }

            user.Password = passwordHasher.HashPassword(user, dto.NewPassword);
            await _userRepository.Update(user);
        }

    }
}
