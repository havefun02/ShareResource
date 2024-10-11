using ShareResource.Models.Entities;

namespace ShareResource.Models.Dtos
{
    public class UpdateRoleDto
    {
        public string RoleName { set; get; }=string.Empty;
        public string RoleDescription { set; get; }=string.Empty ;
        public List<string>? RolePermissionsId { get; set; }=new List<string>();

    }
}
