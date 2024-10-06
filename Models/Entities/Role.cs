using System.Security;

namespace ShareResource.Models.Entities
{
    public class Role
    {
        public string? RoleId {set;get;}
        public string? RoleName {set;get;} 
        public string? RoleDescription {set;get;}
        public virtual ICollection<User>? RoleUsers { set; get; }
        public virtual ICollection<RolePermission>? RolePermissions { set;get;} 
    }
}
