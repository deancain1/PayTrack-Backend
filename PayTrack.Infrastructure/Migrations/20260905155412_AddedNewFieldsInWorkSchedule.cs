using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedNewFieldsInWorkSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_CompanyAdminId",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_CompanyAdminId",
                table: "WorkSchedules");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleName",
                table: "WorkSchedules",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<int>(
                name: "GracePeriodMinutes",
                table: "WorkSchedules",
                type: "integer",
                nullable: false,
                defaultValue: 15,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AbsentCutoffTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "interval");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                table: "WorkSchedules",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<TimeOnly>(
                name: "EarliestStartTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "LatestStartTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ApplicationUserId",
                table: "WorkSchedules",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_WorkSchedules_ScheduleName",
                table: "WorkSchedules",
                column: "ScheduleName");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_ApplicationUserId",
                table: "WorkSchedules",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkSchedules_AspNetUsers_ApplicationUserId",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_ApplicationUserId",
                table: "WorkSchedules");

            migrationBuilder.DropIndex(
                name: "IX_WorkSchedules_ScheduleName",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "EarliestStartTime",
                table: "WorkSchedules");

            migrationBuilder.DropColumn(
                name: "LatestStartTime",
                table: "WorkSchedules");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WorkSchedules",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<string>(
                name: "ScheduleName",
                table: "WorkSchedules",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<int>(
                name: "GracePeriodMinutes",
                table: "WorkSchedules",
                type: "integer",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer",
                oldDefaultValue: 15);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "WorkSchedules",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AbsentCutoffTime",
                table: "WorkSchedules",
                type: "interval",
                nullable: false,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");

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
    }
}
