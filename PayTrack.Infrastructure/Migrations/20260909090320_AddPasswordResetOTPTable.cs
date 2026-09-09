using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPasswordResetOTPTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PasswordResetOTPs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: true),
                    OtpCode = table.Column<string>(type: "character varying(6)", maxLength: 6, nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsUsed = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetOTPs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PasswordResetOTPs_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetOTPs_Email",
                table: "PasswordResetOTPs",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_PasswordResetOTPs_UserId",
                table: "PasswordResetOTPs",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetOTPs");
        }
    }
}
