using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DoAn_Web.Migrations
{
    /// <inheritdoc />
    public partial class AddLegalBases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Create LegalBases table if not exists
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.LegalBases', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.LegalBases (
        LegalID INT IDENTITY(1,1) PRIMARY KEY,
        Title NVARCHAR(255) NOT NULL,
        ReferenceCode NVARCHAR(50) NULL,
        IssuedDate DATE NULL,
        IssuedBy NVARCHAR(255) NULL,
        Category NVARCHAR(50) NULL,
        Description NVARCHAR(MAX) NULL,
        DocumentUrl NVARCHAR(500) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
");

            // Create CompanyLegalBases
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.CompanyLegalBases', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.CompanyLegalBases (
        CompanyID INT NOT NULL,
        LegalID INT NOT NULL,
        ComplianceStatus NVARCHAR(20) DEFAULT 'unknown',
        VerifiedDate DATE NULL,
        Notes NVARCHAR(MAX) NULL,
        CONSTRAINT PK_CompanyLegalBases PRIMARY KEY (CompanyID, LegalID),
        CONSTRAINT FK_CompanyLegalBases_Companies FOREIGN KEY (CompanyID) REFERENCES dbo.Companies(CompanyID),
        CONSTRAINT FK_CompanyLegalBases_LegalBases FOREIGN KEY (LegalID) REFERENCES dbo.LegalBases(LegalID)
    );
END
");

            // Create JobPostingLegalBases
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.JobPostingLegalBases', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.JobPostingLegalBases (
        JobID INT NOT NULL,
        LegalID INT NOT NULL,
        ComplianceStatus NVARCHAR(20) DEFAULT 'unknown',
        CheckedDate DATE NULL,
        Notes NVARCHAR(MAX) NULL,
        CONSTRAINT PK_JobPostingLegalBases PRIMARY KEY (JobID, LegalID),
        CONSTRAINT FK_JobPostingLegalBases_JobPostings FOREIGN KEY (JobID) REFERENCES dbo.JobPostings(JobID),
        CONSTRAINT FK_JobPostingLegalBases_LegalBases FOREIGN KEY (LegalID) REFERENCES dbo.LegalBases(LegalID)
    );
END
");

            // Create InternshipLegalBases
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.InternshipLegalBases', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.InternshipLegalBases (
        InternshipID INT NOT NULL,
        LegalID INT NOT NULL,
        ComplianceStatus NVARCHAR(20) DEFAULT 'compliant',
        AppliedDate DATE DEFAULT GETDATE(),
        Notes NVARCHAR(MAX) NULL,
        CONSTRAINT PK_InternshipLegalBases PRIMARY KEY (InternshipID, LegalID),
        CONSTRAINT FK_InternshipLegalBases_Internships FOREIGN KEY (InternshipID) REFERENCES dbo.Internships(InternshipID),
        CONSTRAINT FK_InternshipLegalBases_LegalBases FOREIGN KEY (LegalID) REFERENCES dbo.LegalBases(LegalID)
    );
END
");

            // Indexes
            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_LegalBases_Category')
BEGIN
    CREATE INDEX IX_LegalBases_Category ON dbo.LegalBases(Category);
END");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_CompanyLegalBases_CompanyID')
BEGIN
    CREATE INDEX IX_CompanyLegalBases_CompanyID ON dbo.CompanyLegalBases(CompanyID);
END");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_JobPostingLegalBases_JobID')
BEGIN
    CREATE INDEX IX_JobPostingLegalBases_JobID ON dbo.JobPostingLegalBases(JobID);
END");

            migrationBuilder.Sql(@"IF NOT EXISTS (SELECT name FROM sys.indexes WHERE name = 'IX_InternshipLegalBases_InternshipID')
BEGIN
    CREATE INDEX IX_InternshipLegalBases_InternshipID ON dbo.InternshipLegalBases(InternshipID);
END");

            // Optional sample data (safe inserts if not exist)
            migrationBuilder.Sql(@"
IF NOT EXISTS (SELECT 1 FROM dbo.LegalBases WHERE ReferenceCode = '59/2020/QH14')
BEGIN
    INSERT INTO dbo.LegalBases (Title, ReferenceCode, IssuedDate, IssuedBy, Category, Description, DocumentUrl)
    VALUES (N'Luật Doanh nghiệp 2020', N'59/2020/QH14', '2020-06-17', N'Quốc hội', N'company', N'Quy định về thành lập và hoạt động của doanh nghiệp.', N'https://thuvienphapluat.vn/van-ban/Doanh-nghiep/Luat-Doanh-nghiep-2020-445015.aspx');
END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop in reverse order if they exist
            migrationBuilder.Sql(@"
IF OBJECT_ID(N'dbo.InternshipLegalBases', N'U') IS NOT NULL
    DROP TABLE dbo.InternshipLegalBases;
IF OBJECT_ID(N'dbo.JobPostingLegalBases', N'U') IS NOT NULL
    DROP TABLE dbo.JobPostingLegalBases;
IF OBJECT_ID(N'dbo.CompanyLegalBases', N'U') IS NOT NULL
    DROP TABLE dbo.CompanyLegalBases;
IF OBJECT_ID(N'dbo.LegalBases', N'U') IS NOT NULL
    DROP TABLE dbo.LegalBases;
");
        }
    }
}
