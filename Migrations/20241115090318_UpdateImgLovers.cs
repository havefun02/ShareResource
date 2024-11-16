using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShareResource.Migrations
{
    /// <inheritdoc />
    public partial class UpdateImgLovers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImgLovers_Users_ImgId",
                table: "ImgLovers");

            migrationBuilder.AddForeignKey(
                name: "FK_ImgLovers_Users_UserId",
                table: "ImgLovers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImgLovers_Users_UserId",
                table: "ImgLovers");

            migrationBuilder.AddForeignKey(
                name: "FK_ImgLovers_Users_ImgId",
                table: "ImgLovers",
                column: "ImgId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
