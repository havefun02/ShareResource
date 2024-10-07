using Microsoft.AspNetCore.Identity;
using ShareResource.Interfaces;
namespace ShareResource.Models.Entities
{
    public class User
    {
        public string? UserId {  get; set; }
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public string? UserPhone { get; set; }
        public string? UserRoleId {  get; set; }
        public virtual Role? UserRole {  get; set; }
        public virtual Token? UserToken {  get; set; }
        

    }
}
