using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;
using System.Linq;

namespace YourNamespace.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public NotificationsController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách thông báo của sinh viên
        public async Task<IActionResult> Index()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem thông báo.";
                return RedirectToAction("Login", "Home");
            }

            var notifications = await _context.Notifications
                .Where(n => n.UserId == studentId && n.UserType == "student")
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }

        // Các action khác (MarkAsRead, Delete) giữ nguyên
    }
}