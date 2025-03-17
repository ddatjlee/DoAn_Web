using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;

namespace DoAn_Web.Controllers
{
    public class NotificationsController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public NotificationsController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Action để đánh dấu thông báo đã đọc
        public async Task<IActionResult> MarkAsRead(int id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            if (notification == null)
            {
                TempData["ErrorMessage"] = "Thông báo không tồn tại.";
                return RedirectToAction("Index");
            }

            // Cập nhật trạng thái thông báo thành đã đọc
            notification.IsRead = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Thông báo đã được đánh dấu là đã đọc.";
            return RedirectToAction("Index");
        }

        // Action để xóa thông báo
        public async Task<IActionResult> Delete(int id)
        {
            var notification = await _context.Notifications
                .FirstOrDefaultAsync(n => n.NotificationId == id);

            if (notification != null)
            {
                _context.Notifications.Remove(notification);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thông báo đã được xóa.";
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông báo cần xóa.";
            }

            return RedirectToAction("Index");
        }

        // Action để hiển thị tất cả thông báo
        public async Task<IActionResult> Index()
        {
            var notifications = await _context.Notifications
                .Where(n => n.UserId == 1) // Thay đổi theo logic người dùng
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();

            return View(notifications);
        }
    }
}
