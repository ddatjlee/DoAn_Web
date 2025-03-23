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
        public async Task<IActionResult> ViewInterviews(int jobId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra xem công việc có thuộc về công ty này không
            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.JobId == jobId && j.CompanyId == companyId.Value);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Công việc không tồn tại hoặc không thuộc quyền quản lý của bạn.";
                return RedirectToAction("Index", "Home");
            }

            var interviews = await _context.Interviews
                .Include(i => i.Application)
                .ThenInclude(a => a.Student)
                .Include(i => i.Application)
                .ThenInclude(a => a.JobPostings)
                .Where(i => i.Application.JobId == jobId)
                .ToListAsync();

            ViewBag.JobTitle = job.Title;
            ViewBag.JobId = jobId;
            return View(interviews);
        }
        [HttpGet]
        public async Task<IActionResult> SelectJobForApplications()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("LoginCompany", "Home");
            }

            var jobs = await _context.JobPostings
                .Where(j => j.CompanyId == companyId.Value)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            if (!jobs.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa đăng công việc nào.";
            }

            return View(jobs);
        }

        [HttpGet]
        public async Task<IActionResult> SelectJobForInterview()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("LoginCompany", "Home");
            }

            var jobs = await _context.JobPostings
                .Where(j => j.CompanyId == companyId.Value)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            if (!jobs.Any())
            {
                TempData["ErrorMessage"] = "Bạn chưa đăng công việc nào.";
            }

            return View(jobs);
        }
        [HttpGet]
        public async Task<IActionResult> ViewApplications(int jobId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra xem công việc có thuộc về công ty này không
            var job = await _context.JobPostings
                .FirstOrDefaultAsync(j => j.JobId == jobId && j.CompanyId == companyId.Value);
            if (job == null)
            {
                TempData["ErrorMessage"] = "Công việc không tồn tại hoặc không thuộc quyền quản lý của bạn.";
                return RedirectToAction("Index", "Home");
            }

            var applications = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.JobPostings)
                .Where(a => a.JobId == jobId && a.Status != "rejected")
                .ToListAsync();

            ViewBag.JobTitle = job.Title;
            return View(applications);
        }
        [HttpGet]
        public async Task<IActionResult> ScheduleInterview(int applicationId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Index", "Home");
            }

            var application = await _context.Applications
                .Include(a => a.JobPostings)
                .ThenInclude(j => j.Company)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null || application.JobPostings == null || application.Student == null || application.JobPostings.CompanyId != companyId.Value)
            {
                TempData["ErrorMessage"] = "Ứng tuyển không tồn tại, không có thông tin công việc/sinh viên, hoặc không thuộc công ty của bạn.";
                return RedirectToAction("Index", "Home");
            }

            var existingInterview = await _context.Interviews
                .FirstOrDefaultAsync(i => i.ApplicationId == applicationId);
            if (existingInterview != null)
            {
                TempData["ErrorMessage"] = "Ứng viên này đã được lên lịch phỏng vấn.";
                return RedirectToAction("ViewApplications", new { jobId = application.JobId });
            }

            ViewBag.StudentName = application.Student.FullName;
            ViewBag.JobTitle = application.JobPostings.Title;
            ViewBag.JobId = application.JobId;

            var interview = new Interview
            {
                ApplicationId = applicationId,
                // Không gán Application
                StartTime = DateTime.Now.AddDays(1),
                EndTime = DateTime.Now.AddDays(1).AddHours(1),
                InterviewType = "online"
            };
            return View(interview);
        }

        [HttpPost]
        public async Task<IActionResult> ScheduleInterview(Interview interview)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Index", "Home");
            }

            var application = await _context.Applications
                .Include(a => a.JobPostings)
                .ThenInclude(j => j.Company)
                .Include(a => a.Student)
                .FirstOrDefaultAsync(a => a.ApplicationId == interview.ApplicationId);

            if (application == null || application.JobPostings == null || application.Student == null || application.JobPostings.CompanyId != companyId.Value)
            {
                TempData["ErrorMessage"] = "Ứng tuyển không tồn tại, không có thông tin công việc/sinh viên, hoặc không thuộc công ty của bạn.";
                return RedirectToAction("Index", "Home");
            }

            if (interview.StartTime < DateTime.Now)
            {
                ModelState.AddModelError("StartTime", "Thời gian bắt đầu phải từ hiện tại trở đi.");
            }

            if (interview.EndTime < DateTime.Now)
            {
                ModelState.AddModelError("EndTime", "Thời gian kết thúc phải từ hiện tại trở đi.");
            }

            if (interview.StartTime >= interview.EndTime)
            {
                ModelState.AddModelError("EndTime", "Thời gian kết thúc phải sau thời gian bắt đầu.");
            }

            if (interview.InterviewType == "online" && string.IsNullOrEmpty(interview.OnlineLink))
            {
                ModelState.AddModelError("OnlineLink", "Vui lòng cung cấp link phỏng vấn online.");
            }

            if (interview.InterviewType == "in-person" && string.IsNullOrEmpty(interview.Location))
            {
                ModelState.AddModelError("Location", "Vui lòng cung cấp địa điểm phỏng vấn.");
            }

            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                TempData["ErrorMessage"] = "Có lỗi xảy ra: " + string.Join("; ", errors);

                ViewBag.StudentName = application.Student.FullName;
                ViewBag.JobTitle = application.JobPostings.Title;
                ViewBag.JobId = application.JobId;
                return View(interview);
            }

            try
            {
                interview.Result = "pending";
                _context.Interviews.Add(interview);
                application.Status = "reviewing";
                _context.Applications.Update(application);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    UserId = application.StudentId,
                    UserType = "student",
                    Message = $"Bạn được mời phỏng vấn cho vị trí {application.JobPostings.Title} vào {interview.StartTime:dd/MM/yyyy HH:mm}.",
                    IsRead = false,
                    CreatedAt = DateTime.Now
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Lịch phỏng vấn đã được gửi thành công!";
                return RedirectToAction("ViewApplications", new { jobId = application.JobId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi lưu lịch phỏng vấn: {ex.Message}";
                ViewBag.StudentName = application.Student.FullName;
                ViewBag.JobTitle = application.JobPostings.Title;
                ViewBag.JobId = application.JobId;
                return View(interview);
            }
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