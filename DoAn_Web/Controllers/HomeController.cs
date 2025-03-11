using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Data;
using DoAn_Web.Models;
using System.Security.Cryptography;
using System.Text;

namespace DoAn_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public HomeController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Trang chủ
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách công việc mới nhất
            var jobs = await _context.JobPostings
                .Include(j => j.Company) // Lấy thông tin công ty
                .Where(j => j.IsActive == true) // Chỉ lấy công việc đang tuyển
                .OrderByDescending(j => j.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Lấy danh sách công ty
            var companies = await _context.Companies
                .OrderByDescending(c => c.CreatedAt)
                .Take(5)
                .ToListAsync();

            // Lấy danh sách ngành nghề
            var industries = await _context.Industries.ToListAsync();

            // Kiểm tra xem người dùng có đăng nhập không
            ViewBag.UserName = HttpContext.Session.GetString("UserName");

            // Gửi dữ liệu qua View bằng ViewModel
            var model = new HomeViewModel
            {
                Jobs = jobs,
                Companies = companies,
                Industries = industries
            };

            return View(model);
        }

        // Trang đăng nhập (GET)
        public IActionResult Login()
        {
            return View();
        }

        // Xử lý đăng nhập (POST)
        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var user = await _context.Students.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null || user.PasswordHash != password)
            {
                ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng!";
                return View();
            }

            // Lưu trạng thái đăng nhập bằng Session
            HttpContext.Session.SetString("UserEmail", user.Email);
            HttpContext.Session.SetString("UserName", user.FullName);

            return RedirectToAction("Index");
        }

        // Đăng xuất
        public IActionResult Logout()
        {
            HttpContext.Session.Clear(); // Xóa session đăng nhập
            return RedirectToAction("Login");
        }
        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Register(string email, string password,string confirmPassword,string fullName)
        {
            // Kiểm tra email đã tồn tại chưa
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu nhập lại không khớp!";
                return View();
            }

            // Kiểm tra email đã tồn tại chưa
            var existingUser = await _context.Students.FirstOrDefaultAsync(u => u.Email == email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Email đã tồn tại!";
                return View();
            }

            // Thêm tài khoản vào database mà không mã hóa mật khẩu
            var newUser = new Student
            {
                Email = email,
                PasswordHash = password, // Lưu trực tiếp mật khẩu không mã hóa
                FullName = fullName,
                University = "Chưa cập nhật",
                Major = "Chưa cập nhật",
                CreatedAt = DateTime.Now
            };

            _context.Students.Add(newUser);
            await _context.SaveChangesAsync();

            // Chuyển hướng đến trang đăng nhập sau khi đăng ký thành công
            return RedirectToAction("Login", "Home");
        }
    }
}
