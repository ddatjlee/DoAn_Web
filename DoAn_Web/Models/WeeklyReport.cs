using System;

namespace DoAn_Web.Models
{
    public class WeeklyReport
    {
        public int ReportId { get; set; }
        public int InternshipId { get; set; }
        public int WeekNumber { get; set; }
        public DateTime ReportDate { get; set; }
        public string Content { get; set; }
        public string Status { get; set; } = "pending";
        public string? SupervisorComment { get; set; }
        public DateTime? ReviewedAt { get; set; }

        // Navigation property
        public virtual Internship Internship { get; set; }
    }
}