using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace DoAn_Web.Controllers
{
    public class JobsController : Controller
    {
        private readonly RecruitmentSystemContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public JobsController(RecruitmentSystemContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }
        [HttpGet]
        public IActionResult CreateJob()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Index", "Home");
            }

            ViewBag.JobTypes = _context.JobTypes.ToList();
            ViewBag.ExperienceLevels = _context.ExperienceLevels.ToList();
            ViewBag.Locations = _context.Locations.ToList();
            ViewBag.Skills = _context.Skills.ToList(); // Thêm danh sách kỹ năng
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobPosting jobPosting, int[] selectedSkills)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);  // In lỗi ra console để kiểm tra
                }
            }

            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Index", "Home");
            }

            jobPosting.CompanyId = companyId.Value;
            jobPosting.CreatedAt = DateTime.Now;
            jobPosting.UpdatedAt = DateTime.Now;
            jobPosting.IsActive = true;
            jobPosting.IsApproved = false;  // Đặt IsApproved là false khi tạo tin

            // In giá trị của jobPosting để kiểm tra trước khi lưu
            Console.WriteLine($"Job Title: {jobPosting.Title}");
            Console.WriteLine($"Salary Range: {jobPosting.SalaryRange}");

            // Lưu công việc vào cơ sở dữ liệu
            _context.JobPostings.Add(jobPosting);
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                UserId = companyId.Value,  // Gửi thông báo cho công ty này
                UserType = "company",
                Message = $"Bài đăng '{jobPosting.Title}' của bạn đang đợi admin duyệt.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng tin tuyển dụng thành công! Đợi admin duyệt.";
            return RedirectToAction("Index","Home");
        }

    }
}