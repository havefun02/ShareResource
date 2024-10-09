using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore.Update.Internal;
using ShareResource.Models.Entities;
using ShareResource.Models.Dtos;

namespace ShareResource.Interfaces
{
    public interface IAuthService<Entity,Token> 
    {
        public Task<Token> UpdateTokenAsync(Token token);
        public Task<Token> GetTokenInfoAsync(string token);
        public Task<(string,string)> Login(LoginDto user);
        public Task<Entity> Register(RegisterDto user);
        public Task<bool> Logout(string userId);
        public Task<bool> UpdatePassword(UpdatePasswordDto dto, string userId);
        public Task<bool> ChangePassword(ChangePasswordDto dto);
    }
}
