using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSeedMethod : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: "Execute");

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Delete", "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Read", "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Write", "Admin" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Read", "Guest" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "FullPermissions", "Owner" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Read", "User" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Write", "User" });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "6dcdd4f7-5982-4cac-be6c-789c16713220");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "9de61b5f-3e9a-4d12-84c1-4201918aa7ca");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: "Delete");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: "FullPermissions");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: "Read");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: "Write");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: "Admin");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: "Guest");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: "Owner");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: "User");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "PermissionName" },
                values: new object[,]
                {
                    { "Delete", "Delete" },
                    { "Execute", "Execute" },
                    { "FullPermissions", "FullPermissions" },
                    { "Read", "Read" },
                    { "Write", "Write" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleDescription", "RoleName" },
                values: new object[,]
                {
                    { "Admin", null, "Admin" },
                    { "Guest", null, "Guest" },
                    { "Owner", null, "Owner" },
                    { "User", null, "User" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { "Delete", "Admin" },
                    { "Read", "Admin" },
                    { "Write", "Admin" },
                    { "Read", "Guest" },
                    { "FullPermissions", "Owner" },
                    { "Read", "User" },
                    { "Write", "User" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserIcon", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "6dcdd4f7-5982-4cac-be6c-789c16713220", "Admin@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEPdb2Kds9zaV+lZAgLts+BsDA2ckBhZCjvyE6o9b2DJxaGXrtCsp6CC5m1GSPvyzrQ==", "123456789", "Admin" },
                    { "9de61b5f-3e9a-4d12-84c1-4201918aa7ca", "Owner@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEDxsM2qbk/gD7VbJLFYJotV5P3AWaieGJ0QerztVWfKn/QCTIOr7fvgciYKhOogPRQ==", "123456789", "Owner" }
                });
        }
    }
}
