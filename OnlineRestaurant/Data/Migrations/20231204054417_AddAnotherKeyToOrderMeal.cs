using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddAnotherKeyToOrderMeal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderMeals",
                table: "OrderMeals");

            migrationBuilder.DropIndex(
                name: "IX_OrderMeals_OrderId",
                table: "OrderMeals");

            migrationBuilder.AlterColumn<string>(
                name: "Addition",
                table: "OrderMeals",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderMeals",
                table: "OrderMeals",
                columns: new[] { "OrderId", "MealId", "Addition" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMeals_MealId",
                table: "OrderMeals",
                column: "MealId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrderMeals",
                table: "OrderMeals");

            migrationBuilder.DropIndex(
                name: "IX_OrderMeals_MealId",
                table: "OrderMeals");

            migrationBuilder.AlterColumn<string>(
                name: "Addition",
                table: "OrderMeals",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrderMeals",
                table: "OrderMeals",
                columns: new[] { "MealId", "OrderId" });

            migrationBuilder.CreateIndex(
                name: "IX_OrderMeals_OrderId",
                table: "OrderMeals",
                column: "OrderId");
        }
    }
}
