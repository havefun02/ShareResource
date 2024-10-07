namespace ShareResource.Models.Dtos
{
    public class RoleResultDto
    {
        public string? RoleName { get; set; }
        public List<PermissionResultDto>? UserPermissions { get; set; }

    }
}
