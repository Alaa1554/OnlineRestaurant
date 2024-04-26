using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OnlineRestaurant.Migrations
{
    /// <inheritdoc />
    public partial class AddChoicesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choice_MealAdditions_MealAdditionId",
                table: "Choice");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choice",
                table: "Choice");

            migrationBuilder.RenameTable(
                name: "Choice",
                newName: "Choices");

            migrationBuilder.RenameIndex(
                name: "IX_Choice_MealAdditionId",
                table: "Choices",
                newName: "IX_Choices_MealAdditionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choices",
                table: "Choices",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Choices_MealAdditions_MealAdditionId",
                table: "Choices",
                column: "MealAdditionId",
                principalTable: "MealAdditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Choices_MealAdditions_MealAdditionId",
                table: "Choices");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Choices",
                table: "Choices");

            migrationBuilder.RenameTable(
                name: "Choices",
                newName: "Choice");

            migrationBuilder.RenameIndex(
                name: "IX_Choices_MealAdditionId",
                table: "Choice",
                newName: "IX_Choice_MealAdditionId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Choice",
                table: "Choice",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Choice_MealAdditions_MealAdditionId",
                table: "Choice",
                column: "MealAdditionId",
                principalTable: "MealAdditions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
