using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;

namespace DoAn_Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public AdminController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Trang Dashboard cho admin (Quản lý bài đăng tuyển dụng)
        public async Task<IActionResult> Dashboard()
        {
            var jobPostings = await _context.JobPostings
                .Where(j => j.IsApproved == false) // Lọc bài đăng chưa duyệt
                .Include(j => j.Company)  // Thêm thông tin công ty
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(jobPostings);
        }

        // Duyệt bài đăng tuyển dụng
        [HttpPost]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            var job = await _context.JobPostings.FindAsync(jobId);
            if (job != null)
            {
                // Đánh dấu bài đăng là đã duyệt
                job.IsApproved = true;
                _context.Update(job);
                await _context.SaveChangesAsync();

                // Tạo thông báo cho công ty
                var notification = new Notification
                {
                    UserId = job.CompanyId,  // Gửi thông báo cho công ty này
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

            return NotFound();
        }

    }

}

