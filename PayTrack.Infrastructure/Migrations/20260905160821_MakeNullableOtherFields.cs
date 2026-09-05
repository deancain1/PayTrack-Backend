using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PayTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MakeNullableOtherFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AbsentCutoffTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<TimeSpan>(
                name: "StartTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "EndTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "AbsentCutoffTime",
                table: "WorkSchedules",
                type: "time without time zone",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time without time zone",
                oldNullable: true);
        }
    }
}
