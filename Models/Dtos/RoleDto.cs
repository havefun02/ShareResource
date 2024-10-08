using ShareResource.Models.Entities;

namespace ShareResource.Models.Dtos
{
    public class RoleDto
    {
        public string RoleId { get; set; }=string.Empty;
        public string RoleName { get; set; }=string.Empty;
        public List<Permission>? RolePermissions { get; set; }

    }
}
