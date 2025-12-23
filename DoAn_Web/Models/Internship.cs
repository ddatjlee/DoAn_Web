using System;
using System.Collections.Generic;

namespace DoAn_Web.Models
{
    public class Internship
    {
        public int InternshipId { get; set; }
        public int StudentId { get; set; }
        public int CompanyId { get; set; }
        public int SupervisorId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string Status { get; set; } = "Đang thực tập";
        public string? InternshipReportUrl { get; set; } // Đường dẫn file báo cáo thực tập

        // Navigation properties
        public virtual Student Student { get; set; }
        public virtual Company Company { get; set; }
        public virtual Supervisor Supervisor { get; set; }
        public virtual ICollection<WeeklyReport> WeeklyReports { get; set; } = new List<WeeklyReport>();
        public virtual CompanyEvaluation? CompanyEvaluation { get; set; }
        public virtual SupervisorEvaluation? SupervisorEvaluation { get; set; }
    }
}