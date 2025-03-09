using System;
using System.Collections.Generic;

namespace DoAn_Web.Models;

public partial class Student
{
    public int StudentId { get; set; }

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string FullName { get; set; } = null!;

    public string? AvatarUrl { get; set; }

    public DateOnly? DateOfBirth { get; set; }

    public string? Phone { get; set; }

    public string University { get; set; } = null!;

    public string Major { get; set; } = null!;

    public decimal? Gpa { get; set; }

    public int? GraduationYear { get; set; }

    public string? LinkedinProfile { get; set; }

    public string? GithubProfile { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}
