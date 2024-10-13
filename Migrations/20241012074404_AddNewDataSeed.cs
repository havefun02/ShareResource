using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class AddNewDataSeed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "6c2f3621-2ee8-4a5f-ba60-d5dc5b319eb5");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "d7786ace-9762-44d5-ae2d-f982c5f07428");

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "PermissionId", "PermissionName" },
                values: new object[,]
                {
                    { "Delete", "Delete" },
                    { "Execute", "Execute" }
                });

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "RoleId", "RoleDescription", "RoleName" },
                values: new object[] { "User", null, "User" });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "419c2037-2a38-4d6a-8b6a-9f6edd4379f5", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEN7RXAXV5YV/bMDnRFtystRnzqOXS2QiW1KrLrepsOJEBUI2+EVNOLQh+AJBVfwpeA==", "123456789", "Admin" },
                    { "99b88640-2b16-4604-bc52-c9ceb3127e0c", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAENGEZzz2Cz343rl61sMUwoo/SEVwXGiY+3b6oEkapQ2QTboTV+qLs1MqLMG4WJVipA==", "123456789", "Owner" }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { "Delete", "Admin" },
                    { "Read", "User" },
                    { "Write", "User" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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
                keyValues: new object[] { "Read", "User" });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { "Write", "User" });

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "419c2037-2a38-4d6a-8b6a-9f6edd4379f5");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "99b88640-2b16-4604-bc52-c9ceb3127e0c");

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "PermissionId",
                keyValue: "Delete");

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "RoleId",
                keyValue: "User");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "6c2f3621-2ee8-4a5f-ba60-d5dc5b319eb5", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEHDMRKZgo7Z+G0wov0uD5ln9/Fz3aupQlOeCrTcwUVr4fsYaqV2srYggLG5wgICYTg==", "123456789", "Owner" },
                    { "d7786ace-9762-44d5-ae2d-f982c5f07428", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEHSQlgMGYAdoXhWA1KoXnKgW66/YG38IYfpRHh8OTdayY0FsxCpGGfSrcrGvObsU0Q==", "123456789", "Admin" }
                });
        }
    }
}
