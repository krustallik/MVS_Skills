using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MVC.Migrations
{
    /// <inheritdoc />
    public partial class Roles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData("AspNetRoles",
                ["Name", "NormalizedName"],
                new object[] { "Admin", "ADMIN" });

            migrationBuilder.InsertData("AspNetRoles",
                ["Name", "NormalizedName"],
                new object[] { "User", "USER" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
