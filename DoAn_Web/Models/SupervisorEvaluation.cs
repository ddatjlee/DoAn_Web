using System;

namespace DoAn_Web.Models
{
    public class SupervisorEvaluation
    {
        public int EvaluationID { get; set; }  // Changed from EvaluationId to match the error
        public int InternshipId { get; set; }
        public DateTime EvaluationDate { get; set; }
        public int Score { get; set; }  // Changed from FinalScore to match the error
        public string? Comments { get; set; }

        // Navigation property
        public virtual Internship Internship { get; set; }
    }
}