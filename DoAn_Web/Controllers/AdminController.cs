using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;
using System.Linq;

namespace DoAn_Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public AdminController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> About()
        {
            return View();
        }

        // Danh sách đơn được chấp nhận chờ phân công
        public async Task<IActionResult> AssignInternships()
        {
            // Kiểm tra đăng nhập admin
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("LoginAdmin", "Home");
            }

            var acceptedApplications = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Where(a => a.Status == "Accepted" && 
                       !_context.Internships.Any(i => i.StudentId == a.StudentId))
                .ToListAsync();

            ViewBag.Supervisors = await _context.Supervisors.ToListAsync();

            return View(acceptedApplications);
        }

        // Xử lý phân công thực tập
        [HttpPost]
        public async Task<IActionResult> AssignInternship(int applicationId, int supervisorId, DateTime startDate)
        {
            // Kiểm tra đăng nhập admin
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("LoginAdmin", "Home");
            }

            var application = await _context.Applications
                .Include(a => a.Student)
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .FirstOrDefaultAsync(a => a.ApplicationId == applicationId);

            if (application == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy đơn ứng tuyển.";
                return RedirectToAction(nameof(AssignInternships));
            }

            // Kiểm tra xem sinh viên đã được phân công chưa
            var existingInternship = await _context.Internships
                .FirstOrDefaultAsync(i => i.StudentId == application.StudentId);

            if (existingInternship != null)
            {
                TempData["ErrorMessage"] = "Sinh viên này đã được phân công thực tập.";
                return RedirectToAction(nameof(AssignInternships));
            }

            // Tạo bản ghi thực tập mới
            var newInternship = new Internship
            {
                StudentId = application.StudentId,
                CompanyId = application.Job.CompanyId,
                SupervisorId = supervisorId,
                StartDate = startDate,
                EndDate = startDate.AddMonths(3), // Thêm EndDate mặc định là 3 tháng
                Status = "Đang thực tập"
            };

            _context.Internships.Add(newInternship);

            // Cập nhật trạng thái đơn
            application.Status = "assigned"; // Hoặc trạng thái phù hợp khác

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã phân công giảng viên hướng dẫn thành công.";
            return RedirectToAction(nameof(AssignInternships));
            application.Status = "Completed";

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Phân công thực tập thành công!";
            return RedirectToAction("AssignInternships");
        }

        public async Task<IActionResult> Dashboard()
        {
            var jobPostings = await _context.JobPostings
                .Include(j => j.Company)
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            var jobIds = jobPostings.Select(j => j.JobId).ToList();
            var approvalHistories = await _context.ApprovalHistories
                .Where(h => jobIds.Contains(h.JobId))
                .ToDictionaryAsync(h => h.JobId, h => h);

            var viewModel = jobPostings.Select(job => new JobPostingViewModel
            {
                Job = job,
                History = approvalHistories.ContainsKey(job.JobId) ? approvalHistories[job.JobId] : null
            }).ToList();

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            var job = await _context.JobPostings.FindAsync(jobId);
            if (job == null)
            {
                return NotFound();
            }

            var existingHistory = await _context.ApprovalHistories
                .AnyAsync(h => h.JobId == jobId);
            if (existingHistory)
            {
                TempData["ErrorMessage"] = "Bài đăng này đã được xử lý.";
                return RedirectToAction("Dashboard");
            }

            job.IsApproved = true;
            _context.Update(job);

            var approvalHistory = new ApprovalHistory
            {
                JobId = jobId,
                Action = "Approved",
                ActionDate = DateTime.Now,
                AdminId = HttpContext.Session.GetInt32("AdminId") ?? 0
            };
            _context.ApprovalHistories.Add(approvalHistory);

            var notification = new Notification
            {
                UserId = job.CompanyId,
                UserType = "company",
                Message = $"Bài đăng '{job.Title}' của bạn đã được duyệt.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bài đăng đã được duyệt thành công!";
            return RedirectToAction("Dashboard");
        }

        [HttpPost]
        public async Task<IActionResult> RejectJob(int jobId)
        {
            var job = await _context.JobPostings.FindAsync(jobId);
            if (job == null)
            {
                return NotFound();
            }

            var existingHistory = await _context.ApprovalHistories
                .AnyAsync(h => h.JobId == jobId);
            if (existingHistory)
            {
                TempData["ErrorMessage"] = "Bài đăng này đã được xử lý.";
                return RedirectToAction("Dashboard");
            }

            // Lưu lịch sử từ chối
            var approvalHistory = new ApprovalHistory
            {
                JobId = jobId,
                Action = "Rejected",
                ActionDate = DateTime.Now,
                AdminId = HttpContext.Session.GetInt32("AdminId") ?? 0
            };
            _context.ApprovalHistories.Add(approvalHistory);

            // Tạo thông báo cho công ty
            var notification = new Notification
            {
                UserId = job.CompanyId,
                UserType = "company",
                Message = $"Bài đăng '{job.Title}' của bạn đã bị từ chối.",
                IsRead = false,
                CreatedAt = DateTime.Now
            };
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Bài đăng đã bị từ chối thành công!";
            return RedirectToAction("Dashboard");
        }
        public async Task<IActionResult> ManageAccounts()
        {
            var companies = await _context.Companies
                .Select(c => new AccountViewModel
                {
                    Id = c.CompanyId,
                    Name = c.Name,
                    Email = c.Email,
                    Type = "Company",
                    IsLocked = c.IsLocked
                })
                .ToListAsync();

            var students = await _context.Students
                .Select(s => new AccountViewModel
                {
                    Id = s.StudentId,
                    Name = s.FullName,
                    Email = s.StudentCode,
                    Type = "Student",
                    IsLocked = s.IsLocked
                })
                .ToListAsync();

            var accounts = companies.Concat(students).OrderBy(a => a.Type).ThenBy(a => a.Name).ToList();

            return View(accounts);
        }

        // Action khóa tài khoản
        [HttpPost]
        public async Task<IActionResult> LockAccount(int id, string type)
        {
            if (type == "Company")
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound();
                }
                company.IsLocked = true;
                _context.Update(company);
            }
            else if (type == "Student")
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return NotFound();
                }
                student.IsLocked = true;
                _context.Update(student);
            }
            else
            {
                return BadRequest("Loại tài khoản không hợp lệ.");
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tài khoản đã bị khóa thành công!";
            return RedirectToAction("ManageAccounts");
        }

        // Action mở khóa tài khoản
        [HttpPost]
        public async Task<IActionResult> UnlockAccount(int id, string type)
        {
            if (type == "Company")
            {
                var company = await _context.Companies.FindAsync(id);
                if (company == null)
                {
                    return NotFound();
                }
                company.IsLocked = false;
                _context.Update(company);
            }
            else if (type == "Student")
            {
                var student = await _context.Students.FindAsync(id);
                if (student == null)
                {
                    return NotFound();
                }
                student.IsLocked = false;
                _context.Update(student);
            }
            else
            {
                return BadRequest("Loại tài khoản không hợp lệ.");
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Tài khoản đã được mở khóa thành công!";
            return RedirectToAction("ManageAccounts");
        }
    }
    public class JobPostingViewModel
    {
        public JobPosting Job { get; set; }
        public ApprovalHistory History { get; set; }
    }
    public class AccountViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Type { get; set; } // "Company" hoặc "Student"
        public bool IsLocked { get; set; }
    }
}

