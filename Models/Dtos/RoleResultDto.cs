namespace ShareResource.Models.Dtos
{
    public class RoleResultDto
    {
        public string? Role { get; set; }
        public List<PermissionResultDto>? Permissions { get; set; }

    }
}
