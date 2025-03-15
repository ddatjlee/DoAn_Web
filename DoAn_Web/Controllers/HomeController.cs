using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DoAn_Web.Models;

namespace YourNamespace.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public HomeController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Trang chủ hiển thị danh sách việc làm và công ty nổi bật
        public async Task<IActionResult> Index()
        {
            var jobs = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location) // Include Location để hiển thị địa điểm
                .Include(j => j.Skills)  // Include Skills để hiển thị kỹ năng yêu cầu
                .Where(j => j.IsActive == true) // Chỉ lấy các công việc đang hoạt động
                .OrderByDescending(j => j.CreatedAt)
                .Take(3) // Lấy 3 công việc mới nhất
                .ToListAsync();

            var companies = await _context.Companies
                .Where(c => c.Verified == true) // Chỉ lấy các công ty đã được xác minh
                .OrderByDescending(c => c.CreatedAt)
                .Take(4) // Lấy 4 công ty nổi bật
                .ToListAsync();

            var model = new HomeViewModel
            {
                Jobs = jobs,
                Companies = companies
            };

            return View(model);
        }

        // Action tìm kiếm công ty
        public async Task<IActionResult> SearchCompany(string companyName)
        {
            var companies = await _context.Companies
                .Where(c => string.IsNullOrEmpty(companyName) || c.Name.Contains(companyName))
                .ToListAsync();

            var jobs = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.Skills)
                .Where(j => j.IsActive == true)
                .OrderByDescending(j => j.CreatedAt)
                .Take(3)
                .ToListAsync();

            var model = new HomeViewModel
            {
                Jobs = jobs,
                Companies = companies
            };

            return View("Index", model);
        }

        // Các action khác (Login, Register, Logout)
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}