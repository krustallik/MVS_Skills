using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    /// <inheritdoc />
    public partial class UserInfoUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "UserInfos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "UserInfos",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_OwnerId",
                table: "UserInfos",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_UserInfos_UserId",
                table: "UserInfos",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_AspNetUsers_OwnerId",
                table: "UserInfos",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserInfos_AspNetUsers_UserId",
                table: "UserInfos",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_AspNetUsers_OwnerId",
                table: "UserInfos");

            migrationBuilder.DropForeignKey(
                name: "FK_UserInfos_AspNetUsers_UserId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_OwnerId",
                table: "UserInfos");

            migrationBuilder.DropIndex(
                name: "IX_UserInfos_UserId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "UserInfos");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserInfos");
        }
    }
}
