using ShareResource.Models.Entities;

namespace ShareResource.Models.Dtos
{
    public class UpdateRoleDto
    {
        public string? RoleName { set; get; }
        public string? RoleDescription { set; get; }
        public List<Permission>? RolePermissions { get; set; }

    }
}
