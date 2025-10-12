using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateWeeklyReports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ReviewedAt",
                table: "WeeklyReports",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SupervisorComment",
                table: "WeeklyReports",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Internships",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReviewedAt",
                table: "WeeklyReports");

            migrationBuilder.DropColumn(
                name: "SupervisorComment",
                table: "WeeklyReports");

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Internships",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
