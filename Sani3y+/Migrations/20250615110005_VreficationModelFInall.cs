using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Sani3y_.Migrations
{
    /// <inheritdoc />
    public partial class VreficationModelFInall : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VerificationRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CraftsmanId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CardImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsRejected = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VerificationRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VerificationRequests_AspNetUsers_CraftsmanId",
                        column: x => x.CraftsmanId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "CardImagePath", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "GoogleId", "Governorate", "IsTrusted", "LastName", "Location", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "ProfessionId", "ProfileImagePath", "RefreshToken", "RefreshTokenExpiryTime", "Role", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[] { "7ab3d2a4-b6ae-480e-80e2-5b97c54e5f33", 0, null, "98212b0c-77d1-4bb4-9293-e307ff6718d6", "admin@sanai3yplus.com", true, "Admin", null, "", null, "User", "", true, null, "ADMIN@SANAI3YPLUS.COM", "ADMIN@EXAMPLE.COM", "AQAAAAIAAYagAAAAEJfhTswhLI5PHIqSW0TEObECD0w5YUBpybAois77tfo+ANCAiCxsXoCymyuOopZ6fA==", null, false, null, null, "zQg3XUt1Rf0pdQP0TIW5XvySUIxmb339ZDOx0Q69OiI=", new DateTime(2025, 5, 22, 9, 56, 56, 993, DateTimeKind.Utc).AddTicks(8356), "Admin", "RKVK74DEVATHOYBU7MWSMQM2I6R2EAJE", false, "admin@example.com" });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[] { "Adminn", "7ab3d2a4-b6ae-480e-80e2-5b97c54e5f33" });

            migrationBuilder.CreateIndex(
                name: "IX_VerificationRequests_CraftsmanId",
                table: "VerificationRequests",
                column: "CraftsmanId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VerificationRequests");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "Adminn", "7ab3d2a4-b6ae-480e-80e2-5b97c54e5f33" });

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "7ab3d2a4-b6ae-480e-80e2-5b97c54e5f33");
        }
    }
}
