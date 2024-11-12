using Microsoft.AspNetCore.Identity.Data;
using Microsoft.EntityFrameworkCore.Update.Internal;
using ShareResource.Models.Entities;
using ShareResource.Models.Dtos;

namespace ShareResource.Interfaces
{
    public interface IAuthService<Entity> 
    {

        public Task<(string,string)> Login(LoginDto user);
        public Task<bool> Register(RegisterDto user);
        public Task<bool> Logout(string userId);
        public Task<bool> UpdatePassword(UpdatePasswordDto dto, string userId);
        public Task<bool> ChangePassword(ChangePasswordDto dto);
    }
}
