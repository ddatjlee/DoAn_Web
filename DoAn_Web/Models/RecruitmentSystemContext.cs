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
    public virtual DbSet<Company> Companies { get; set; }
    public virtual DbSet<ExperienceLevel> ExperienceLevels { get; set; }
    public virtual DbSet<Interview> Interviews { get; set; }
    public virtual DbSet<JobPosting> JobPostings { get; set; }
    public virtual DbSet<JobType> JobTypes { get; set; }
    public virtual DbSet<Location> Locations { get; set; }
    public virtual DbSet<Notification> Notifications { get; set; }
    public virtual DbSet<Skill> Skills { get; set; }
    public virtual DbSet<Student> Students { get; set; }
    public virtual DbSet<StudentSkill> StudentSkills { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Data Source=DESKTOP-F2I8231\\SQLEXPRESS;Initial Catalog=RecruitmentSystem;Integrated Security=True;Connect Timeout=30;Encrypt=True;Trust Server Certificate=True;Application Intent=ReadWrite;Multi Subnet Failover=False");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Admin>(entity =>
        {
            entity.HasKey(e => e.AdminId).HasName("PK__Admins__719FE4E83F0ADBD6");
            entity.HasIndex(e => e.Email, "UQ__Admins__A9D10534275F7E2B").IsUnique();
            entity.Property(e => e.AdminId).HasColumnName("AdminID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
        });

        modelBuilder.Entity<Application>(entity =>
        {
            entity.HasKey(e => e.ApplicationId).HasName("PK__Applicat__C93A4F79C22B7651");
            entity.HasIndex(e => new { e.JobId, e.StudentId }, "UC_JobStudent").IsUnique();
            entity.HasIndex(e => e.Status, "idx_ApplicationStatus");
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.AppliedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.ResumeUrl).HasMaxLength(512);
            entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("pending");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");

            entity.HasOne(d => d.Job).WithMany(p => p.Applications)
                .HasForeignKey(d => d.JobId)
                .HasConstraintName("FK__Applicati__JobID__6383C8BA");

            entity.HasOne(d => d.Student).WithMany(p => p.Applications)
                .HasForeignKey(d => d.StudentId)
                .HasConstraintName("FK__Applicati__Stude__6477ECF3");
        });

        modelBuilder.Entity<Company>(entity =>
        {
            entity.HasKey(e => e.CompanyId).HasName("PK__Companie__2D971C4CF5C3DFB9");
            entity.HasIndex(e => e.TaxCode, "UQ__Companie__12945A28EA2B5918").IsUnique();
            entity.HasIndex(e => e.Email, "UQ__Companie__A9D10534AA270B37").IsUnique();
            entity.HasIndex(e => e.Name, "idx_CompanyName");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Email).HasMaxLength(255);
            entity.Property(e => e.LogoUrl).HasMaxLength(512);
            entity.Property(e => e.Name).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.TaxCode).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Verified).HasDefaultValue(false);
            entity.Property(e => e.Website).HasMaxLength(255);
        });

        modelBuilder.Entity<ExperienceLevel>(entity =>
        {
            entity.HasKey(e => e.LevelId).HasName("PK__Experien__09F03C06F90F0825");
            entity.HasIndex(e => e.Name, "UQ__Experien__737584F6C37D1958").IsUnique();
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(e => e.InterviewId).HasName("PK__Intervie__C97C5832B2936BBB");
            entity.Property(e => e.InterviewId).HasColumnName("InterviewID");
            entity.Property(e => e.ApplicationId).HasColumnName("ApplicationID");
            entity.Property(e => e.InterviewType).HasMaxLength(20);
            entity.Property(e => e.OnlineLink).HasMaxLength(512);
            entity.Property(e => e.Result).HasMaxLength(20).HasDefaultValue("pending");

            entity.HasOne(d => d.Application).WithMany(p => p.Interviews)
                .HasForeignKey(d => d.ApplicationId)
                .HasConstraintName("FK__Interview__Appli__6A30C649");
        });

        modelBuilder.Entity<JobPosting>(entity =>
        {
            entity.HasKey(e => e.JobId).HasName("PK__JobPosti__056690E2A4CF24D8");
            entity.HasIndex(e => e.Title, "idx_JobTitle");
            entity.Property(e => e.JobId).HasColumnName("JobID");
            entity.Property(e => e.CompanyId).HasColumnName("CompanyID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsActive).HasDefaultValue(true);
            entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");
            entity.Property(e => e.LevelId).HasColumnName("LevelID");
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.SalaryRange).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(255);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.Vacancies).HasDefaultValue(1);

            entity.HasOne(d => d.Company).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.CompanyId)
                .HasConstraintName("FK__JobPostin__Compa__5629CD9C");

            entity.HasOne(d => d.JobType).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.JobTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JobPostin__JobTy__571DF1D5");

            entity.HasOne(d => d.Level).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.LevelId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__JobPostin__Level__5812160E");

            entity.HasOne(d => d.Location).WithMany(p => p.JobPostings)
                .HasForeignKey(d => d.LocationId)
                .HasConstraintName("FK__JobPostin__Locat__59063A47");
        });

        modelBuilder.Entity<JobType>(entity =>
        {
            entity.HasKey(e => e.JobTypeId).HasName("PK__JobTypes__E1F4624DEF01B53A");
            entity.HasIndex(e => e.Name, "UQ__JobTypes__737584F6DA67F25C").IsUnique();
            entity.Property(e => e.JobTypeId).HasColumnName("JobTypeID");
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Location>(entity =>
        {
            entity.HasKey(e => e.LocationId).HasName("PK__Location__E7FEA477784C5274");
            entity.HasIndex(e => new { e.City, e.Country }, "UC_CityCountry").IsUnique();
            entity.Property(e => e.LocationId).HasColumnName("LocationID");
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.Country).HasMaxLength(100);
        });

        modelBuilder.Entity<Notification>(entity =>
        {
            entity.HasKey(e => e.NotificationId).HasName("PK__Notifica__20CF2E32DED69B73");
            entity.Property(e => e.NotificationId).HasColumnName("NotificationID");
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.IsRead).HasDefaultValue(false);
            entity.Property(e => e.UserId).HasColumnName("UserID");
            entity.Property(e => e.UserType).HasMaxLength(20);
        });

        modelBuilder.Entity<Skill>(entity =>
        {
            entity.HasKey(e => e.SkillId).HasName("PK__Skills__DFA091E77D367F23");
            entity.HasIndex(e => e.Name, "UQ__Skills__737584F65BE6A9E3").IsUnique();
            entity.Property(e => e.SkillId).HasColumnName("SkillID");
            entity.Property(e => e.Name).HasMaxLength(100);
        });

        modelBuilder.Entity<Student>(entity =>
        {
            entity.HasKey(e => e.StudentId).HasName("PK__Students__32C52A798449AFDC");
            entity.HasIndex(e => e.StudentCode, "UQ__Students__1FC88604551BEB52").IsUnique();
            entity.HasIndex(e => e.StudentCode, "idx_StudentCode");
            entity.Property(e => e.StudentId).HasColumnName("StudentID");
            entity.Property(e => e.AvatarUrl).HasMaxLength(512);
            entity.Property(e => e.CreatedAt).HasDefaultValueSql("(getdate())");
            entity.Property(e => e.FullName).HasMaxLength(255);
            entity.Property(e => e.GitHubProfile).HasMaxLength(255);
            entity.Property(e => e.Gpa).HasColumnType("decimal(3, 2)").HasColumnName("GPA");
            entity.Property(e => e.PasswordHash).HasMaxLength(255);
            entity.Property(e => e.Phone).HasMaxLength(20);
            entity.Property(e => e.StudentCode).HasMaxLength(20);
            entity.Property(e => e.UpdatedAt).HasDefaultValueSql("(getdate())");

            entity.HasMany(s => s.Skills)
                .WithMany(s => s.Students)
                .UsingEntity<StudentSkill>(
                    j => j
                        .HasOne(ss => ss.Skill)
                        .WithMany()
                        .HasForeignKey(ss => ss.SkillID),
                    j => j
                        .HasOne(ss => ss.Student)
                        .WithMany(s => s.StudentSkills)
                        .HasForeignKey(ss => ss.StudentID)
                        .OnDelete(DeleteBehavior.Cascade),
                    j =>
                    {
                        j.HasKey(ss => new { ss.StudentID, ss.SkillID }); // Định nghĩa khóa chính composite
                        j.ToTable("StudentSkills");
                        j.Property(ss => ss.StudentID).HasColumnName("StudentID");
                        j.Property(ss => ss.SkillID).HasColumnName("SkillID");
                    });
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}