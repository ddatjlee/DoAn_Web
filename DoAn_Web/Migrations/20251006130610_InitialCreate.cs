using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Web.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Admins",
                columns: table => new
                {
                    AdminID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Admins__719FE4E85095AB1B", x => x.AdminID);
                });

            migrationBuilder.CreateTable(
                name: "Companies",
                columns: table => new
                {
                    CompanyID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    TaxCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Website = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LogoUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Verified = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Companie__2D971C4CAA3C5901", x => x.CompanyID);
                });

            migrationBuilder.CreateTable(
                name: "ExperienceLevels",
                columns: table => new
                {
                    LevelID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Experien__09F03C06B75D802C", x => x.LevelID);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                columns: table => new
                {
                    JobTypeID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobTypes__E1F4624D6814BF47", x => x.JobTypeID);
                });

            migrationBuilder.CreateTable(
                name: "Locations",
                columns: table => new
                {
                    LocationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Location__E7FEA4775C7DA5D7", x => x.LocationID);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    NotificationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserID = table.Column<int>(type: "int", nullable: false),
                    UserType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: true, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Notifica__20CF2E32B7D83E7F", x => x.NotificationID);
                });

            migrationBuilder.CreateTable(
                name: "Skills",
                columns: table => new
                {
                    SkillID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Skills__DFA091E7F4A7D538", x => x.SkillID);
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    AvatarUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true),
                    GPA = table.Column<decimal>(type: "decimal(3,2)", nullable: true),
                    GraduationYear = table.Column<int>(type: "int", nullable: true),
                    GitHubProfile = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    IsLocked = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Students__32C52A796631424E", x => x.StudentID);
                });

            migrationBuilder.CreateTable(
                name: "Supervisors",
                columns: table => new
                {
                    SupervisorId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Phone = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Position = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Department = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Supervisors", x => x.SupervisorId);
                });

            migrationBuilder.CreateTable(
                name: "JobPostings",
                columns: table => new
                {
                    JobID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyID = table.Column<int>(type: "int", nullable: false),
                    JobTypeID = table.Column<int>(type: "int", nullable: false),
                    LevelID = table.Column<int>(type: "int", nullable: false),
                    LocationID = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Requirements = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Benefits = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SalaryRange = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ApplicationDeadline = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Vacancies = table.Column<int>(type: "int", nullable: true, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "bit", nullable: true, defaultValue: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    IsApproved = table.Column<bool>(type: "bit", nullable: true, defaultValue: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobPosti__056690E2335C22CF", x => x.JobID);
                    table.ForeignKey(
                        name: "FK__JobPostin__Compa__5629CD9C",
                        column: x => x.CompanyID,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__JobPostin__JobTy__571DF1D5",
                        column: x => x.JobTypeID,
                        principalTable: "JobTypes",
                        principalColumn: "JobTypeID");
                    table.ForeignKey(
                        name: "FK__JobPostin__Level__5812160E",
                        column: x => x.LevelID,
                        principalTable: "ExperienceLevels",
                        principalColumn: "LevelID");
                    table.ForeignKey(
                        name: "FK__JobPostin__Locat__59063A47",
                        column: x => x.LocationID,
                        principalTable: "Locations",
                        principalColumn: "LocationID");
                });

            migrationBuilder.CreateTable(
                name: "StudentSkills",
                columns: table => new
                {
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__StudentS__5F3F2367CC559E16", x => new { x.StudentID, x.SkillID });
                    table.ForeignKey(
                        name: "FK__StudentSk__Skill__4F7CD00D",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID");
                    table.ForeignKey(
                        name: "FK__StudentSk__Stude__4E88ABD4",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Internships",
                columns: table => new
                {
                    InternshipId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Internships", x => x.InternshipId);
                    table.ForeignKey(
                        name: "FK_Internships_Companies_CompanyId",
                        column: x => x.CompanyId,
                        principalTable: "Companies",
                        principalColumn: "CompanyID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Internships_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Internships_Supervisors_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Supervisors",
                        principalColumn: "SupervisorId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Applications",
                columns: table => new
                {
                    ApplicationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobID = table.Column<int>(type: "int", nullable: false),
                    StudentID = table.Column<int>(type: "int", nullable: false),
                    CoverLetter = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ResumeUrl = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false, defaultValue: "pending"),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "(getdate())"),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Applicat__C93A4F7913F8B7FF", x => x.ApplicationID);
                    table.ForeignKey(
                        name: "FK__Applicati__JobID__6383C8BA",
                        column: x => x.JobID,
                        principalTable: "JobPostings",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__Applicati__Stude__6477ECF3",
                        column: x => x.StudentID,
                        principalTable: "Students",
                        principalColumn: "StudentID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ApprovalHistory",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JobId = table.Column<int>(type: "int", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ActionDate = table.Column<DateTime>(type: "datetime", nullable: false),
                    AdminId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Approval__3214EC077D38655D", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApprovalHistory_JobPostings",
                        column: x => x.JobId,
                        principalTable: "JobPostings",
                        principalColumn: "JobID");
                });

            migrationBuilder.CreateTable(
                name: "JobSkills",
                columns: table => new
                {
                    JobID = table.Column<int>(type: "int", nullable: false),
                    SkillID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__JobSkill__689C99FC8E1DCDF3", x => new { x.JobID, x.SkillID });
                    table.ForeignKey(
                        name: "FK__JobSkills__JobID__5BE2A6F2",
                        column: x => x.JobID,
                        principalTable: "JobPostings",
                        principalColumn: "JobID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK__JobSkills__Skill__5CD6CB2B",
                        column: x => x.SkillID,
                        principalTable: "Skills",
                        principalColumn: "SkillID");
                });

            migrationBuilder.CreateTable(
                name: "CompanyEvaluations",
                columns: table => new
                {
                    EvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternshipID = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InternshipId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyEvaluations", x => x.EvaluationID);
                    table.ForeignKey(
                        name: "FK_CompanyEvaluations_Internships",
                        column: x => x.InternshipID,
                        principalTable: "Internships",
                        principalColumn: "InternshipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CompanyEvaluations_Internships_InternshipId1",
                        column: x => x.InternshipId1,
                        principalTable: "Internships",
                        principalColumn: "InternshipId");
                });

            migrationBuilder.CreateTable(
                name: "SupervisorEvaluations",
                columns: table => new
                {
                    EvaluationID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternshipID = table.Column<int>(type: "int", nullable: false),
                    EvaluationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Score = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    InternshipId1 = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupervisorEvaluations", x => x.EvaluationID);
                    table.ForeignKey(
                        name: "FK_SupervisorEvaluations_Internships",
                        column: x => x.InternshipID,
                        principalTable: "Internships",
                        principalColumn: "InternshipId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupervisorEvaluations_Internships_InternshipId1",
                        column: x => x.InternshipId1,
                        principalTable: "Internships",
                        principalColumn: "InternshipId");
                });

            migrationBuilder.CreateTable(
                name: "WeeklyReports",
                columns: table => new
                {
                    ReportId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InternshipId = table.Column<int>(type: "int", nullable: false),
                    WeekNumber = table.Column<int>(type: "int", nullable: false),
                    ReportDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Content = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WeeklyReports", x => x.ReportId);
                    table.ForeignKey(
                        name: "FK_WeeklyReports_Internships_InternshipId",
                        column: x => x.InternshipId,
                        principalTable: "Internships",
                        principalColumn: "InternshipId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    InterviewID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ApplicationID = table.Column<int>(type: "int", nullable: false),
                    InterviewType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OnlineLink = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Result = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, defaultValue: "pending")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Intervie__C97C5832B9354D02", x => x.InterviewID);
                    table.ForeignKey(
                        name: "FK__Interview__Appli__6A30C649",
                        column: x => x.ApplicationID,
                        principalTable: "Applications",
                        principalColumn: "ApplicationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "UQ__Admins__A9D1053400F486A2",
                table: "Admins",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_ApplicationStatus",
                table: "Applications",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_StudentID",
                table: "Applications",
                column: "StudentID");

            migrationBuilder.CreateIndex(
                name: "UC_JobStudent",
                table: "Applications",
                columns: new[] { "JobID", "StudentID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ApprovalHistory_JobId",
                table: "ApprovalHistory",
                column: "JobId");

            migrationBuilder.CreateIndex(
                name: "idx_CompanyName",
                table: "Companies",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "UQ__Companie__12945A28F1C72148",
                table: "Companies",
                column: "TaxCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Companie__A9D105341070757D",
                table: "Companies",
                column: "Email",
                unique: true);

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

            migrationBuilder.CreateIndex(
                name: "UQ__Experien__737584F69D146EA7",
                table: "ExperienceLevels",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Internships_CompanyId",
                table: "Internships",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_StudentId",
                table: "Internships",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_Internships_SupervisorId",
                table: "Internships",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ApplicationID",
                table: "Interviews",
                column: "ApplicationID");

            migrationBuilder.CreateIndex(
                name: "idx_JobTitle",
                table: "JobPostings",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_CompanyID",
                table: "JobPostings",
                column: "CompanyID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_JobTypeID",
                table: "JobPostings",
                column: "JobTypeID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_LevelID",
                table: "JobPostings",
                column: "LevelID");

            migrationBuilder.CreateIndex(
                name: "IX_JobPostings_LocationID",
                table: "JobPostings",
                column: "LocationID");

            migrationBuilder.CreateIndex(
                name: "IX_JobSkills_SkillID",
                table: "JobSkills",
                column: "SkillID");

            migrationBuilder.CreateIndex(
                name: "UQ__JobTypes__737584F62860435D",
                table: "JobTypes",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UC_CityCountry",
                table: "Locations",
                columns: new[] { "City", "Country" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Skills__737584F6EF42AF77",
                table: "Skills",
                column: "Name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "idx_StudentCode",
                table: "Students",
                column: "StudentCode");

            migrationBuilder.CreateIndex(
                name: "UQ__Students__1FC88604308920C0",
                table: "Students",
                column: "StudentCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StudentSkills_SkillID",
                table: "StudentSkills",
                column: "SkillID");

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
                name: "IX_Supervisors_Email",
                table: "Supervisors",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_WeeklyReports_InternshipId",
                table: "WeeklyReports",
                column: "InternshipId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Admins");

            migrationBuilder.DropTable(
                name: "ApprovalHistory");

            migrationBuilder.DropTable(
                name: "CompanyEvaluations");

            migrationBuilder.DropTable(
                name: "Interviews");

            migrationBuilder.DropTable(
                name: "JobSkills");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "StudentSkills");

            migrationBuilder.DropTable(
                name: "SupervisorEvaluations");

            migrationBuilder.DropTable(
                name: "WeeklyReports");

            migrationBuilder.DropTable(
                name: "Applications");

            migrationBuilder.DropTable(
                name: "Skills");

            migrationBuilder.DropTable(
                name: "Internships");

            migrationBuilder.DropTable(
                name: "JobPostings");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "Supervisors");

            migrationBuilder.DropTable(
                name: "Companies");

            migrationBuilder.DropTable(
                name: "JobTypes");

            migrationBuilder.DropTable(
                name: "ExperienceLevels");

            migrationBuilder.DropTable(
                name: "Locations");
        }
    }
}
