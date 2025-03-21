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

        public async Task<IActionResult> Dashboard()
        {
            var jobPostings = await _context.JobPostings
                .Where(j => j.IsApproved == false) 
                .Include(j => j.Company) 
                .OrderByDescending(j => j.CreatedAt)
                .ToListAsync();

            return View(jobPostings);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveJob(int jobId)
        {
            var job = await _context.JobPostings.FindAsync(jobId);
            if (job != null)
            {
                job.IsApproved = true;
                _context.Update(job);
                await _context.SaveChangesAsync();

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

            return NotFound();
        }

    }

}

