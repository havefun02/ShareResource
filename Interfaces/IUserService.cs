using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;

namespace ShareResource.Interfaces
{
    public interface IUserService<T> where T : 
        User
    {
        public Task<T> EditProfile(UserDto userDto);
        public Task DeleteProfile(string userId);
        public Task<T> GetUserProfile(string userId);
    }
    public interface IAdminService<T> where T : User
    {
        public Task UpdateUserRole(UpdateUserRoleDto updateUserRoleDto);
        public Task DeleteUserAccount(string userId);
    }
}
