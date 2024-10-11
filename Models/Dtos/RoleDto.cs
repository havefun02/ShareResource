using ShareResource.Models.Entities;

namespace ShareResource.Models.Dtos
{
    public class RoleDto
    {
        public string RoleName { get; set; }=string.Empty;
        public List<string> RolePermissionsId { get; set; }=new List<string>();

    }
}
