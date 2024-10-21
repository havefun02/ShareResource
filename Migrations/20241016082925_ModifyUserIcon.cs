using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class ModifyUserIcon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "3dca01a7-bc03-4bce-96cb-b9df7a8bfabd");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "c215753d-8be5-4da5-bc87-e14ffdfcce3f");

            migrationBuilder.AlterColumn<byte[]>(
                name: "UserIcon",
                table: "Users",
                type: "longblob",
                nullable: true,
                oldClrType: typeof(byte),
                oldType: "tinyint unsigned",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserIcon", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "6dcdd4f7-5982-4cac-be6c-789c16713220", "Admin@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEPdb2Kds9zaV+lZAgLts+BsDA2ckBhZCjvyE6o9b2DJxaGXrtCsp6CC5m1GSPvyzrQ==", "123456789", "Admin" },
                    { "9de61b5f-3e9a-4d12-84c1-4201918aa7ca", "Owner@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEDxsM2qbk/gD7VbJLFYJotV5P3AWaieGJ0QerztVWfKn/QCTIOr7fvgciYKhOogPRQ==", "123456789", "Owner" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "6dcdd4f7-5982-4cac-be6c-789c16713220");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "9de61b5f-3e9a-4d12-84c1-4201918aa7ca");

            migrationBuilder.AlterColumn<byte>(
                name: "UserIcon",
                table: "Users",
                type: "tinyint unsigned",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "longblob",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserIcon", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "3dca01a7-bc03-4bce-96cb-b9df7a8bfabd", "Admin@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEBFQhPJmJfVOTl1pNYmlb9mQSMTnCr7zVZDPWpaW9GtuA6re1Fmq3+uwbTY2dgW04w==", "123456789", "Admin" },
                    { "c215753d-8be5-4da5-bc87-e14ffdfcce3f", "Owner@gmail.com", null, "Lapphan", "AQAAAAIAAYagAAAAEHtco3ykntSp7yyjrz5GdNSDWNrqKKQwT+Dim33JIBDR2T8Hc+0N7xCtP+ObfW3WPw==", "123456789", "Owner" }
                });
        }
    }
}
