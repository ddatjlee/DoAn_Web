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

    public virtual DbSet<Admin> Admins { get; set; }

    public virtual DbSet<Application> Applications { get; set; }

    public virtual DbSet<ApprovalHistory> ApprovalHistories { get; set; }

    public virtual DbSet<Company> Companies { get; set; }

    public virtual DbSet<ExperienceLevel> ExperienceLevels { get; set; }

    public virtual DbSet<Interview> Interviews { get; set; }

    public virtual DbSet<JobPosting> JobPostings { get; set; }

    public virtual DbSet<JobType> JobTypes { get; set; }

    public virtual DbSet<Location> Locations { get; set; }

    public virtual DbSet<Notification> Notifications { get; set; }

    public virtual DbSet<Skill> Skills { get; set; }

    public virtual DbSet<Student> Students { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost,1433;Database=RecruitmentSystem;User ID=sa;Password=12102004@Hung;TrustServerCertificate=True;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E8C163BD62");

            entity.HasIndex(e => e.Email, "UQ__Admins__A9D105340DA56563").IsUnique();

            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Applicat__C93A4F7924D0CBBE");

            entity.HasIndex(e => new { e.JobId, e.StudentId }, "UC_JobStudent").IsUnique();

            entity.HasIndex(e => e.Status, "idx_ApplicationStatus");

            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.ResumeUrl).HasMaxLength(512);
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasDefaultValue("pending");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK__Applicati__JobID__66603565");

            entity.HasOne(d => d.Student).WithMany(p => p.Applications)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Applicati__Stude__6754599E");
        });

        modelBuilder.Entity<ApprovalHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Approval__3214EC0704926CDE");

            entity.ToTable("ApprovalHistory");

            entity.Property(e => e.Action).HasMaxLength(50);
            entity.Property(e => e.ActionDate).HasColumnType("datetime");

            entity.HasOne(d => d.Job).WithMany(p => p.ApprovalHistories)
                .HasForeignKey(d => d.JobId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ApprovalHistory_JobPostings");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__2D971C4CBE7707A8");

            entity.HasIndex(e => e.TaxCode, "UQ__Companie__12945A284E2EE5A5").IsUnique();

            entity.HasIndex(e => e.Email, "UQ__Companie__A9D1053407884248").IsUnique();

            entity.HasIndex(e => e.Name, "idx_CompanyName");

            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.LogoUrl).HasMaxLength(512);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.TaxCode).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Verified).HasDefaultValue(false);
            entity.Property(e => e.Website).HasMaxLength(255);
        });

        modelBuilder.Entity<ExperienceLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Experien__09F03C0698582D95");

            entity.HasIndex(e => e.Name, "UQ__Experien__737584F6C3C18533").IsUnique();

            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__C97C58321933E8A7");

            entity.Property(e => e.InterviewId).HasColumnName("InterviewID");
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.InterviewType).HasMaxLength(20);
            entity.Property(e => e.OnlineLink).HasMaxLength(512);
            entity.Property(e => e.Result)
                .HasMaxLength(20)
                .HasDefaultValue("pending");

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK__Interview__Appli__6D0D32F4");
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__JobPosti__056690E27BDBC809");

            entity.HasIndex(e => e.Title, "idx_JobTitle");

            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.IsApproved).HasDefaultValue(false);
            entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.SalaryRange).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Vacancies).HasDefaultValue(1);

            entity.HasOne(d => d.Company).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK__JobPostin__Compa__59063A47");

            entity.HasOne(d => d.JobType).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.JobTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JobPostin__JobTy__59FA5E80");

            entity.HasOne(d => d.Level).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JobPostin__Level__5AEE82B9");

            entity.HasOne(d => d.Location).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__JobPostin__Locat__5BE2A6F2");

            entity.HasMany(d => d.Skills).WithMany(p => p.Jobs)
                .UsingEntity<Dictionary<string, object>>(
                    "JobSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__JobSkills__Skill__5FB337D6"),
                    l => l.HasOne<JobPosting>().WithMany()
                        .HasForeignKey("JobId")
                        .HasConstraintName("FK__JobSkills__JobID__5EBF139D"),
                    j =>
                    {
                        j.HasKey("JobId", "SkillId").HasName("PK__JobSkill__689C99FC7F2FF4BC");
                        j.ToTable("JobSkills");
                        j.IndexerProperty<int>("JobId").HasColumnName("JobID");
                        j.IndexerProperty<int>("SkillId").HasColumnName("SkillID");
                    });
        });

        modelBuilder.Entity<JobType>(entity =>
        {
            entity.HasKey(e => e.JobTypeId).HasName("PK__JobTypes__E1F4624DFBE2876F");

            entity.HasIndex(e => e.Name, "UQ__JobTypes__737584F6BCE1C91E").IsUnique();

            entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA47709B8A628");

            entity.HasIndex(e => new { e.City, e.Country }, "UC_CityCountry").IsUnique();

            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32EBDC3AD7");

            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserType).HasMaxLength(20);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__Skills__DFA091E7A61C8582");

            entity.HasIndex(e => e.Name, "UQ__Skills__737584F6C0E1C7AA").IsUnique();

            entity.Property(e => e.SkillId).HasColumnName("SkillID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A79321EA83E");

            entity.HasIndex(e => e.StudentCode, "UQ__Students__1FC886043548F5D4").IsUnique();

            entity.HasIndex(e => e.StudentCode, "idx_StudentCode");

            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.AvatarUrl).HasMaxLength(512);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.GitHubProfile).HasMaxLength(255);
            entity.Property(e => e.Gpa)
                .HasColumnType("decimal(3, 2)")
                .HasColumnName("GPA");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.StudentCode).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasMany(d => d.Skills).WithMany(p => p.Students)
                .UsingEntity<Dictionary<string, object>>(
                    "StudentSkill",
                    r => r.HasOne<Skill>().WithMany()
                        .HasForeignKey("SkillId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__StudentSk__Skill__5165187F"),
                    l => l.HasOne<Student>().WithMany()
                        .HasForeignKey("StudentId")
                        .HasConstraintName("FK__StudentSk__Stude__5070F446"),
                    j =>
                    {
                        j.HasKey("StudentId", "SkillId").HasName("PK__StudentS__5F3F2367161042E8");
                        j.ToTable("StudentSkills");
                        j.IndexerProperty<int>("StudentId").HasColumnName("StudentID");
                        j.IndexerProperty<int>("SkillId").HasColumnName("SkillID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
