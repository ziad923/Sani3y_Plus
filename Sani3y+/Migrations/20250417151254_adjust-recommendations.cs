using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sani3y_.Migrations
{
    /// <inheritdoc />
    public partial class adjustrecommendations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Profession",
                table: "CraftsmanRecommendations");

            migrationBuilder.AddColumn<int>(
                name: "ProfessionId",
                table: "CraftsmanRecommendations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CraftsmanRecommendations_ProfessionId",
                table: "CraftsmanRecommendations",
                column: "ProfessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_CraftsmanRecommendations_Professions_ProfessionId",
                table: "CraftsmanRecommendations",
                column: "ProfessionId",
                principalTable: "Professions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CraftsmanRecommendations_Professions_ProfessionId",
                table: "CraftsmanRecommendations");

            migrationBuilder.DropIndex(
                name: "IX_CraftsmanRecommendations_ProfessionId",
                table: "CraftsmanRecommendations");

            migrationBuilder.DropColumn(
                name: "ProfessionId",
                table: "CraftsmanRecommendations");

            migrationBuilder.AddColumn<string>(
                name: "Profession",
                table: "CraftsmanRecommendations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
