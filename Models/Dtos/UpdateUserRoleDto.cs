using ShareResource.Models.Entities;
namespace ShareResource.Models.Dtos
{
    public class UpdateUserRoleDto
    {
        public string UserId { get; set; }= string.Empty;
        public string RoleId { get; set; }=string.Empty;

    }
}
