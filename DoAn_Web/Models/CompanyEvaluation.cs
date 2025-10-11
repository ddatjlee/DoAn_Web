using System;

namespace DoAn_Web.Models
{
    public class CompanyEvaluation
    {
        public int EvaluationId { get; set; }
        public int InternshipId { get; set; }
        public DateTime EvaluationDate { get; set; }
        
        // Các tiêu chí đánh giá (thang điểm 10)
        public decimal CriteriaCompliance { get; set; } // Chấp hành nội quy, quy định
        public decimal CriteriaTaskPerformance { get; set; } // Thực hiện nhiệm vụ 
        public decimal CriteriaRelationship { get; set; } // Quan hệ, giao tiếp với đồng nghiệp
        
        public int Score { get; set; } // Điểm tổng kết
        public string? Comments { get; set; } // Nhận xét

        // Navigation property
        public virtual Internship Internship { get; set; }
    }
}