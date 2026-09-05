using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixWorkScheduleApplicationUserRelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_ApplicationUserId",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_ApplicationUserId",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "WorkSchedules");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_CompanyAdminId",
                table: "WorkSchedules",
                column: "CompanyAdminId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules",
                column: "CompanyAdminId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_CompanyAdminId",
                table: "WorkSchedules");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "WorkSchedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ApplicationUserId",
                table: "WorkSchedules",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_ApplicationUserId",
                table: "WorkSchedules",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
