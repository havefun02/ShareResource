using ShareResource.Models.Dtos;

namespace ShareResource.Interfaces
{
    public interface IRoleService<TRole>
    {
        public Task<TRole> CreateRole(RoleDto roleDto,string adminId);
        public Task<TRole> UpdateRole(UpdateRoleDto updateRoleDto,string roleId, string adminId);
        public Task<int> DeleteRole(string roleId, string adminId);
        public Task<List<TRole>> GetRoles(string adminId);
    }
}
