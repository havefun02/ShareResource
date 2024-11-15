using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class AddImgLovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserRoleId",
                keyValue: null,
                column: "UserRoleId",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "UserRoleId",
                table: "Users",
                type: "varchar(255)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "varchar(255)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ImgLovers",
                columns: table => new
                {
                    ImgId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    UserId = table.Column<string>(type: "varchar(255)", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImgLovers", x => new { x.UserId, x.ImgId });
                    table.ForeignKey(
                        name: "FK_ImgLovers_Imgs_ImgId",
                        column: x => x.ImgId,
                        principalTable: "Imgs",
                        principalColumn: "ImgId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImgLovers_Users_ImgId",
                        column: x => x.ImgId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_ImgLovers_ImgId",
                table: "ImgLovers",
                column: "ImgId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ImgLovers");

            migrationBuilder.AlterColumn<string>(
                name: "UserRoleId",
                table: "Users",
                type: "varchar(255)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "varchar(255)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
