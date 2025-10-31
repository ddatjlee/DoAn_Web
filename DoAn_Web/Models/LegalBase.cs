using System;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Web.Models
{
    public class LegalBase
    {
        [Key]
        public int LegalID { get; set; }
        public string Title { get; set; } = null!;
        public string? ReferenceCode { get; set; }
        public DateTime? IssuedDate { get; set; }
        public string? IssuedBy { get; set; }
        public string? Category { get; set; }
        public string? Description { get; set; }
        public string? DocumentUrl { get; set; }
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public DateTime CreatedAt { get; set; }
    }
}
