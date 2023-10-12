using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddUserImg : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserImgUrl",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserImgUrl",
                table: "AspNetUsers");
        }
    }
}
