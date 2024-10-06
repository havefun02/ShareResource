namespace ShareResource.Models.Entities
{
    public class RolePermission
    {
        public string? RoleId { get; set; }
        public string? PermissionId { get; set; }
        public virtual Role? Role { get; set; }
        public virtual Permission? Permission { get; set; }

    }
}
