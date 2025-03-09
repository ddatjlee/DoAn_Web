using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DoAn_Web.Models;

public partial class RecruitmentSystemContext : DbContext
{
    public RecruitmentSystemContext()
    {
    }

    public RecruitmentSystemContext(DbContextOptions<RecruitmentSystemContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<ExperienceLevel> ExperienceLevels { get; set; }

    public virtual DbSet<Industry> Industries { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<JobPosting> JobPostings { get; set; }

    public virtual DbSet<JobType> JobTypes { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=DESKTOP-F2I8231\\SQLEXPRESS;Initial Catalog=RecruitmentSystem;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__applicat__3BCBDCF26B55F3EB");

            entity.ToTable("applications");

            entity.HasIndex(e => new { e.JobId, e.StudentId }, "UC_JobStudent").IsUnique();

            entity.HasIndex(e => e.Status, "idx_application_status");

            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.AppliedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("applied_at");
            entity.Property(e => e.CoverLetter).HasColumnName("cover_letter");
            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.ResumeUrl)
                .HasMaxLength(512)
                .HasColumnName("resume_url");
            entity.Property(e => e.ReviewedAt).HasColumnName("reviewed_at");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("status");
            entity.Property(e => e.StudentId).HasColumnName("student_id");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK__applicati__job_i__6FE99F9F");

            entity.HasOne(d => d.Student).WithMany(p => p.Applications)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__applicati__stude__70DDC3D8");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__companie__3E267235D82B8FAD");

            entity.ToTable("companies");

            entity.HasIndex(e => e.TaxCode, "UQ__companie__13E39CF6025CCFDF").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__companie__AB6E61642B3B5CB9").IsUnique();

            entity.HasIndex(e => e.Name, "idx_company_name");

            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.LogoUrl)
                .HasMaxLength(512)
                .HasColumnName("logo_url");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.TaxCode)
                .HasMaxLength(20)
                .HasColumnName("tax_code");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.Verified)
                .HasDefaultValue(false)
                .HasColumnName("verified");
            entity.Property(e => e.Website)
                .HasMaxLength(255)
                .HasColumnName("website");
        });

        modelBuilder.Entity<ExperienceLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__experien__034616437B634404");

            entity.ToTable("experience_levels");

            entity.HasIndex(e => e.Name, "UQ__experien__72E12F1BE5A63DBA").IsUnique();

            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Industry>(entity =>
        {
            entity.HasKey(e => e.IndustryId).HasName("PK__industri__A9676AC8DF623FC7");

            entity.ToTable("industries");

            entity.HasIndex(e => e.Name, "UQ__industri__72E12F1BBEF6FD81").IsUnique();

            entity.Property(e => e.IndustryId).HasColumnName("industry_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__intervie__141E555246E8B920");

            entity.ToTable("interviews");

            entity.Property(e => e.InterviewId).HasColumnName("interview_id");
            entity.Property(e => e.ApplicationId).HasColumnName("application_id");
            entity.Property(e => e.EndTime).HasColumnName("end_time");
            entity.Property(e => e.InterviewType)
                .HasMaxLength(20)
                .HasColumnName("interview_type");
            entity.Property(e => e.Location).HasColumnName("location");
            entity.Property(e => e.Notes).HasColumnName("notes");
            entity.Property(e => e.OnlineLink)
                .HasMaxLength(512)
                .HasColumnName("online_link");
            entity.Property(e => e.Result)
                .HasMaxLength(20)
                .HasDefaultValue("pending")
                .HasColumnName("result");
            entity.Property(e => e.StartTime).HasColumnName("start_time");

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK__interview__appli__76969D2E");
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__job_post__6E32B6A52C33229F");

            entity.ToTable("job_postings", tb => tb.HasTrigger("trg_update_job_status"));

            entity.HasIndex(e => e.Title, "idx_job_title");

            entity.Property(e => e.JobId).HasColumnName("job_id");
            entity.Property(e => e.ApplicationDeadline).HasColumnName("application_deadline");
            entity.Property(e => e.Benefits).HasColumnName("benefits");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.IsActive)
                .HasDefaultValue(true)
                .HasColumnName("is_active");
            entity.Property(e => e.JobTypeId).HasColumnName("job_type_id");
            entity.Property(e => e.LevelId).HasColumnName("level_id");
            entity.Property(e => e.Requirements).HasColumnName("requirements");
            entity.Property(e => e.SalaryRange)
                .HasMaxLength(100)
                .HasColumnName("salary_range");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
            entity.Property(e => e.Vacancies)
                .HasDefaultValue(1)
                .HasColumnName("vacancies");

            entity.HasOne(d => d.Company).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK__job_posti__compa__6477ECF3");

            entity.HasOne(d => d.JobType).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.JobTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__job_posti__job_t__656C112C");

            entity.HasOne(d => d.Level).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__job_posti__level__66603565");
        });

        modelBuilder.Entity<JobType>(entity =>
        {
            entity.HasKey(e => e.JobTypeId).HasName("PK__job_type__A8136A7F31A4EE79");

            entity.ToTable("job_types");

            entity.HasIndex(e => e.Name, "UQ__job_type__72E12F1B409E52C8").IsUnique();

            entity.Property(e => e.JobTypeId).HasColumnName("job_type_id");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__location__771831EA6BB1E460");

            entity.ToTable("locations");

            entity.HasIndex(e => new { e.City, e.Country }, "UC_CityCountry").IsUnique();

            entity.Property(e => e.LocationId).HasColumnName("location_id");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__skills__FBBA83792DAB7F76");

            entity.ToTable("skills");

            entity.HasIndex(e => e.Name, "UQ__skills__72E12F1B924CAB21").IsUnique();

            entity.Property(e => e.SkillId).HasColumnName("skill_id");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__students__2A33069A514E734F");

            entity.ToTable("students");

            entity.HasIndex(e => e.Email, "UQ__students__AB6E6164ED344E77").IsUnique();

            entity.Property(e => e.StudentId).HasColumnName("student_id");
            entity.Property(e => e.AvatarUrl)
                .HasMaxLength(512)
                .HasColumnName("avatar_url");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("created_at");
            entity.Property(e => e.DateOfBirth).HasColumnName("date_of_birth");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .HasColumnName("email");
            entity.Property(e => e.FullName)
                .HasMaxLength(255)
                .HasColumnName("full_name");
            entity.Property(e => e.GithubProfile)
                .HasMaxLength(255)
                .HasColumnName("github_profile");
            entity.Property(e => e.Gpa)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("gpa");
            entity.Property(e => e.GraduationYear).HasColumnName("graduation_year");
            entity.Property(e => e.LinkedinProfile)
                .HasMaxLength(255)
                .HasColumnName("linkedin_profile");
            entity.Property(e => e.Major)
                .HasMaxLength(255)
                .HasColumnName("major");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.University)
                .HasMaxLength(255)
                .HasColumnName("university");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnName("updated_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
