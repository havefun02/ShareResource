namespace ShareResource.Models.Entities
{
    public class Permission
    {
        public string? PermissionId {  get; set; }
        public string? PermissionName { get; set; }
        public virtual ICollection<RolePermission>? RolePermissions { set; get; }

    }
}
