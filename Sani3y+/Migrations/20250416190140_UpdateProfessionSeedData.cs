using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sani3y_.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProfessionSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "سبّــاك");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "نــجــار مـســلّح");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "حــــدّاد كـريـتال");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "حــــدّاد مـســلّح");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "بــنّــــاء");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "مُــبيّـض مــحــارة");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 11,
                column: "Name",
                value: "مُــبلّــط ســيرامــيك");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "مُــبلّــط رخـــــام");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "إنــترلـــوك");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 17,
                column: "Name",
                value: "لـــحّـــام");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "رافــعـة أثـــاث");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "ألــومــيتــال");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "سباك");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 5,
                column: "Name",
                value: "نجار مسلح");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 7,
                column: "Name",
                value: "حداد كريتال");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 8,
                column: "Name",
                value: "حداد مسلح");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 9,
                column: "Name",
                value: "بناء");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 10,
                column: "Name",
                value: "مبيض محارة");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 11,
                column: "Name",
                value: "مبلط سيراميك");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 12,
                column: "Name",
                value: "مبلط رخام");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 13,
                column: "Name",
                value: "انترلوك");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 17,
                column: "Name",
                value: "لحام");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 18,
                column: "Name",
                value: "رافعة اثاث");

            migrationBuilder.UpdateData(
                table: "Professions",
                keyColumn: "Id",
                keyValue: 19,
                column: "Name",
                value: "الوميتال");
        }
    }
}
