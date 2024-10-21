using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "419c2037-2a38-4d6a-8b6a-9f6edd4379f5");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "99b88640-2b16-4604-bc52-c9ceb3127e0c");

            migrationBuilder.AddColumn<byte>(
                name: "UserIcon",
                table: "Users",
                type: "tinyint unsigned",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserIcon", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "3dca01a7-bc03-4bce-96cb-b9df7a8bfabd", "Admin@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEBFQhPJmJfVOTl1pNYmlb9mQSMTnCr7zVZDPWpaW9GtuA6re1Fmq3+uwbTY2dgW04w==", "123456789", "Admin" },
                    { "c215753d-8be5-4da5-bc87-e14ffdfcce3f", "Owner@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEHtco3ykntSp7yyjrz5GdNSDWNrqKKQwT+Dim33JIBDR2T8Hc+0N7xCtP+ObfW3WPw==", "123456789", "Owner" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "3dca01a7-bc03-4bce-96cb-b9df7a8bfabd");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "c215753d-8be5-4da5-bc87-e14ffdfcce3f");

            migrationBuilder.DropColumn(
                name: "UserIcon",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "419c2037-2a38-4d6a-8b6a-9f6edd4379f5", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEN7RXAXV5YV/bMDnRFtystRnzqOXS2QiW1KrLrepsOJEBUI2+EVNOLQh+AJBVfwpeA==", "123456789", "Admin" },
                    { "99b88640-2b16-4604-bc52-c9ceb3127e0c", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAENGEZzz2Cz343rl61sMUwoo/SEVwXGiY+3b6oEkapQ2QTboTV+qLs1MqLMG4WJVipA==", "123456789", "Owner" }
                });
        }
    }
}
