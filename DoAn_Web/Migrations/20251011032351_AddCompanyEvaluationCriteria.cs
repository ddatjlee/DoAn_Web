using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCompanyEvaluationCriteria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CompanyEvaluations_Internships_InternshipId1",
                table: "CompanyEvaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_SupervisorEvaluations_Internships_InternshipId1",
                table: "SupervisorEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_SupervisorEvaluations_InternshipID",
                table: "SupervisorEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_SupervisorEvaluations_InternshipId1",
                table: "SupervisorEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_CompanyEvaluations_InternshipID",
                table: "CompanyEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_CompanyEvaluations_InternshipId1",
                table: "CompanyEvaluations");

            migrationBuilder.DropColumn(
                name: "InternshipId1",
                table: "SupervisorEvaluations");

            migrationBuilder.DropColumn(
                name: "InternshipId1",
                table: "CompanyEvaluations");

            migrationBuilder.AddColumn<decimal>(
                name: "CriteriaCompliance",
                table: "CompanyEvaluations",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CriteriaRelationship",
                table: "CompanyEvaluations",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "CriteriaTaskPerformance",
                table: "CompanyEvaluations",
                type: "decimal(3,1)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorEvaluations_InternshipID",
                table: "SupervisorEvaluations",
                column: "InternshipID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEvaluations_InternshipID",
                table: "CompanyEvaluations",
                column: "InternshipID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_SupervisorEvaluations_InternshipID",
                table: "SupervisorEvaluations");

            migrationBuilder.DropIndex(
                name: "IX_CompanyEvaluations_InternshipID",
                table: "CompanyEvaluations");

            migrationBuilder.DropColumn(
                name: "CriteriaCompliance",
                table: "CompanyEvaluations");

            migrationBuilder.DropColumn(
                name: "CriteriaRelationship",
                table: "CompanyEvaluations");

            migrationBuilder.DropColumn(
                name: "CriteriaTaskPerformance",
                table: "CompanyEvaluations");

            migrationBuilder.AddColumn<int>(
                name: "InternshipId1",
                table: "SupervisorEvaluations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "InternshipId1",
                table: "CompanyEvaluations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorEvaluations_InternshipID",
                table: "SupervisorEvaluations",
                column: "InternshipID");

            migrationBuilder.CreateIndex(
                name: "IX_SupervisorEvaluations_InternshipId1",
                table: "SupervisorEvaluations",
                column: "InternshipId1",
                unique: true,
                filter: "[InternshipId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEvaluations_InternshipID",
                table: "CompanyEvaluations",
                column: "InternshipID");

            migrationBuilder.CreateIndex(
                name: "IX_CompanyEvaluations_InternshipId1",
                table: "CompanyEvaluations",
                column: "InternshipId1",
                unique: true,
                filter: "[InternshipId1] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CompanyEvaluations_Internships_InternshipId1",
                table: "CompanyEvaluations",
                column: "InternshipId1",
                principalTable: "Internships",
                principalColumn: "InternshipId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupervisorEvaluations_Internships_InternshipId1",
                table: "SupervisorEvaluations",
                column: "InternshipId1",
                principalTable: "Internships",
                principalColumn: "InternshipId");
        }
    }
}
