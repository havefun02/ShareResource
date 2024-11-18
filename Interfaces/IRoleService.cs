using ShareResource.Models.Dtos;

namespace ShareResource.Interfaces
{
    public interface IRoleService<TRole>
    {
        public Task CreateRole(RoleDto roleDto,string adminId);
        public Task UpdateRole(UpdateRoleDto updateRoleDto,string roleId, string adminId);
        public Task DeleteRole(string roleId, string adminId);
        public Task<List<TRole>> GetRoles(string adminId);
    }
}
