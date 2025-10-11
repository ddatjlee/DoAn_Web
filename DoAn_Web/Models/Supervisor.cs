using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace DoAn_Web.Models
{
    public class Supervisor
    {
        public int SupervisorId { get; set; }

        [Required(ErrorMessage = "Tên là bắt buộc")]
        [Display(Name = "Họ và tên")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [Display(Name = "Số điện thoại")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Vị trí/Chức vụ là bắt buộc")]
        [Display(Name = "Vị trí/Chức vụ")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Khoa/Bộ môn là bắt buộc")]
        [Display(Name = "Khoa/Bộ môn")]
        public string Department { get; set; }

        // Navigation property
        public virtual ICollection<Internship> Internships { get; set; } = new List<Internship>();
    }
}