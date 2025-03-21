using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

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
        public IActionResult CreateInterview(int applicationId)
        {
            // Kiểm tra xem công ty có tin tuyển dụng nào hay không
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (companyId == null)
            {
                return RedirectToAction("LoginCompany", "Home");
            }

            var application = _context.Applications
                .Include(a => a.JobPostings)
                .Include(a => a.Student)
                .FirstOrDefault(a => a.ApplicationId == applicationId && a.JobPostings.CompanyId == companyId);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Công ty chưa có tin tuyển dụng hoặc ứng viên này không phải là ứng viên của công ty.";
                return RedirectToAction("Index", "Home");
            }

            return View(application);
        }

        [HttpPost]
        public async Task<IActionResult> CreateInterview(int applicationId, DateTime startTime, DateTime endTime, string interviewType, string location, string onlineLink, string notes)
        {
            var application = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.JobPostings)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var interview = new Interview
            {
                ApplicationId = applicationId,
                InterviewType = interviewType,
                StartTime = startTime,
                EndTime = endTime,
                Location = location,
                OnlineLink = onlineLink,
                Notes = notes,
                Result = "pending"
            };

            _context.Interviews.Add(interview);
            await _context.SaveChangesAsync();

            // Tạo thông báo cho sinh viên
            var notification = new Notification
            {
                UserId = application.StudentId,
                UserType = "student",
                Message = $"Bạn đã được mời tham gia phỏng vấn cho công việc '{application.JobPostings.Title}' vào lúc {startTime:dd/MM/yyyy HH:mm}.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };

            _context.Notifications.Add(notification);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đơn phỏng vấn đã được gửi thành công!";
            return RedirectToAction("Index", "Home");
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
            ViewBag.Skills = _context.Skills.ToList(); 
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobPosting jobPosting, int[] selectedSkills)
        {
            if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine(error.ErrorMessage);  
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
            jobPosting.IsApproved = false;  

            Console.WriteLine($"Job Title: {jobPosting.Title}");
            Console.WriteLine($"Salary Range: {jobPosting.SalaryRange}");

            _context.JobPostings.Add(jobPosting);
            await _context.SaveChangesAsync();

            var notification = new Notification
            {
                UserId = companyId.Value,
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