using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTokensKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "20fb3813-49b3-414c-b8fe-f46fa42ef6b8");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Tokens",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "TokenId",
                table: "Tokens",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                column: "TokenId");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[] { "061c9d9c-3683-404a-b6c7-447d0cecb855", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEKI4WsfroN4uvTKpN/CfGXXx8h2xMbnOXaNMDUIY0FEdQFq8w7DY1nLi9iVuhP/J1A==", "123456789", "Admin" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "061c9d9c-3683-404a-b6c7-447d0cecb855");

            migrationBuilder.DropColumn(
                name: "TokenId",
                table: "Tokens");

            migrationBuilder.UpdateData(
                table: "Tokens",
                keyColumn: "RefreshToken",
                keyValue: null,
                column: "RefreshToken",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "RefreshToken",
                table: "Tokens",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tokens",
                table: "Tokens",
                column: "RefreshToken");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[] { "20fb3813-49b3-414c-b8fe-f46fa42ef6b8", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEDvg9guHEtguBcvTzrKdahy0knANcGHxEPaYuvfZHIT8fnLTyqIVCwqomDJudc29wg==", "123456789", "Admin" });
        }
    }
}
