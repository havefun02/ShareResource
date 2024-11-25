using ShareResource.Models.Dtos;
using ShareResource.Models.Entities;

namespace ShareResource.Interfaces
{
    public interface IUserService<T> where T : 
        User
    {
        public Task EditProfile(UserDto userDto,string UserId);
        public Task<Img> GetUserFile(string userId,string fileId);

        public Task DeleteProfile(string userId);
        public Task<T> GetUserProfile(string userId);
    }
    public interface IAdminService<T> where T : User
    {

        public Task UpdateUserRole(UpdateUserRoleDto updateUserRoleDto,string adminId, string userId);
        public Task DeleteUserAccount(string adminId, string userId);
    }
}
