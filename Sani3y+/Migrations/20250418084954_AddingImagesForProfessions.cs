using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sani3y_.Migrations
{
    /// <inheritdoc />
    public partial class AddingImagesForProfessions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Professions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 1,
                column: "ImagePath",
                value: "/professions/Lightning.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 2,
                column: "ImagePath",
                value: "/professions/Plumbing.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 3,
                column: "ImagePath",
                value: "/professions/Paint roller.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 4,
                column: "ImagePath",
                value: "/professions/Saw.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 5,
                column: "ImagePath",
                value: "/professions/Carpenter.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 6,
                column: "ImagePath",
                value: "/professions/Door.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 7,
                column: "ImagePath",
                value: "/professions/Mask.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 8,
                column: "ImagePath",
                value: "/professions/Steel.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 9,
                column: "ImagePath",
                value: "/professions/Brickwall.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 10,
                column: "ImagePath",
                value: "/professions/Brick wall.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 11,
                column: "ImagePath",
                value: "/professions/Ceramics.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 12,
                column: "ImagePath",
                value: "/professions/Marble.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 13,
                column: "ImagePath",
                value: "/professions/Tiles.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 14,
                column: "ImagePath",
                value: "/professions/Stone.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 15,
                column: "ImagePath",
                value: "/professions/Steel-1.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 16,
                column: "ImagePath",
                value: "/professions/Wrench.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 17,
                column: "ImagePath",
                value: "/professions/Welding.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 18,
                column: "ImagePath",
                value: "/professions/Crane truck.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 19,
                column: "ImagePath",
                value: "/professions/Window.png");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 20,
                column: "ImagePath",
                value: "/professions/Drywall.png");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Professions");
        }
    }
}
