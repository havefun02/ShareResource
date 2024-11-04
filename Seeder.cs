using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ShareResource.Database;
using ShareResource.Models.Entities;
using System.Reflection.Emit;

namespace ShareResource
{
    public static class Seeder
    {
        public static void Seed(AppDbContext context)
        {
            context.Database.EnsureCreated();
            if (context.Users.Any() && context.Roles.Any() && context.Permissions.Any() && context.RolePermissions.Any())
            {
                return;
            }
            using var transaction=context.Database.BeginTransaction();
            try
            {

                var permisisons = new List<Permission>();
                permisisons.Add(new Permission { PermissionId = "Read", PermissionName = "Read" });
                permisisons.Add(new Permission { PermissionId = "Write", PermissionName = "Write" });
                permisisons.Add(new Permission { PermissionId = "Delete", PermissionName = "Delete" });
                permisisons.Add(new Permission { PermissionId = "Execute", PermissionName = "Execute" });
                permisisons.Add(new Permission { PermissionId = "FullPermissions", PermissionName = "FullPermissions" });
                context.Permissions.AddRange(permisisons);
                context.SaveChanges();


                var roles = new List<Role>();
                roles.Add(new Role { RoleId = "Admin", RoleName = "Admin" });
                roles.Add(new Role { RoleId = "Owner", RoleName = "Owner" });
                roles.Add(new Role { RoleId = "Guest", RoleName = "Guest" });
                roles.Add(new Role { RoleId = "User", RoleName = "User" });
                context.Roles.AddRange(roles);
                context.SaveChanges();


                var rolePermissions = new List<RolePermission>();
                rolePermissions.Add(new RolePermission { RoleId = "Admin", PermissionId = "Read" });
                rolePermissions.Add(new RolePermission { RoleId = "Admin", PermissionId = "Write" });
                rolePermissions.Add(new RolePermission { RoleId = "Admin", PermissionId = "Delete" });

                rolePermissions.Add(new RolePermission { RoleId = "Guest", PermissionId = "Read" });
                rolePermissions.Add(new RolePermission { RoleId = "User", PermissionId = "Read" });
                rolePermissions.Add(new RolePermission { RoleId = "User", PermissionId = "Write" });
                rolePermissions.Add(new RolePermission { RoleId = "Owner", PermissionId = "FullPermissions" });
                context.RolePermissions.AddRange(rolePermissions);
                context.SaveChanges();
                var user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserEmail = "Admin@gmail.com",
                    UserName = "Lapphan",
                    UserPhone = "123456789",
                    UserRoleId = "Admin",
                };
                var ownerUser = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserEmail = "Owner@gmail.com",
                    UserName = "Lapphan",
                    UserPhone = "123456789",
                    UserRoleId = "Owner",
                };
                user.UserPassword = new PasswordHasher<User>().HashPassword(user, "adminpassword");
                ownerUser.UserPassword = new PasswordHasher<User>().HashPassword(ownerUser, "adminpassword");

                context.Users.AddRange([user, ownerUser]);
                context.SaveChanges();

                transaction.Commit();

            }
            catch (Exception ex)
            {
                transaction.Rollback();
                throw new Exception("Error occured while trying to seeding data", ex);
            }
        }
    }
}
