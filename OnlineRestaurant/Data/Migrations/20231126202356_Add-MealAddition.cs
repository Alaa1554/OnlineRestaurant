using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddMealAddition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choice_MealAdditions_MealAdditionId",
                table: "Choice");

            migrationBuilder.AlterColumn<int>(
                name: "MealAdditionId",
                table: "Choice",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Choice_MealAdditions_MealAdditionId",
                table: "Choice",
                column: "MealAdditionId",
                principalTable: "MealAdditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choice_MealAdditions_MealAdditionId",
                table: "Choice");

            migrationBuilder.AlterColumn<int>(
                name: "MealAdditionId",
                table: "Choice",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Choice_MealAdditions_MealAdditionId",
                table: "Choice",
                column: "MealAdditionId",
                principalTable: "MealAdditions",
                principalColumn: "Id");
        }
    }
}
