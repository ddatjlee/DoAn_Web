using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;

namespace DoAn_Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly RecruitmentSystemContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public HomeController(RecruitmentSystemContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index()
        {
            var jobs = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Where(j => j.IsActive == true && j.IsApproved == true) 
                .OrderByDescending(j => j.CreatedAt)
                .Take(3) 
                .ToListAsync();

            var companies = await _context.Companies
                .Where(c => c.Verified == true)
                .OrderByDescending(c => c.CreatedAt)
                .Take(4) 
                .ToListAsync();

            var model = new HomeViewModel
            {
                Jobs = jobs,
                Companies = companies
            };

            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId.HasValue)
            {
                var unreadNotifications = await _context.Notifications
                    .CountAsync(n => n.UserId == studentId.Value && n.UserType == "student" && (n.IsRead.HasValue && !n.IsRead.Value));
                ViewBag.UnreadNotifications = unreadNotifications;
            }
            else
            {
                ViewBag.UnreadNotifications = 0;
            }

            ViewBag.IsLoggedIn = HttpContext.Session.GetInt32("StudentId") != null || HttpContext.Session.GetInt32("CompanyId") != null;

            return View(model);
        }


        public async Task<IActionResult> SearchCompany(string companyName)
        {
            var companies = await _context.Companies
                .Where(c => string.IsNullOrEmpty(companyName) || c.Name.Contains(companyName))
                .ToListAsync();

            var jobs = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
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

        [HttpGet]
        public IActionResult LoginStudent()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginStudent(string studentCode, string password)
        {
            if (string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
            {
                ViewBag.ErrorMessage = "Mã sinh viên không tồn tại.";
                return View();
            }

            if (student.PasswordHash != password)
            {
                ViewBag.ErrorMessage = "Mật khẩu không đúng.";
                return View();
            }

            HttpContext.Session.SetInt32("StudentId", student.StudentId);
            HttpContext.Session.SetString("StudentName", student.FullName);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LoginCompany()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginCompany(string companyEmail, string password)
        {
            if (string.IsNullOrEmpty(companyEmail) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Email == companyEmail);

            if (company == null)
            {
                ViewBag.ErrorMessage = "Email công ty không tồn tại.";
                return View();
            }

            if (company.PasswordHash != password)
            {
                ViewBag.ErrorMessage = "Mật khẩu không đúng.";
                return View();
            }

            HttpContext.Session.SetInt32("CompanyId", company.CompanyId);
            HttpContext.Session.SetString("CompanyName", company.Name);

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult LoginAdmin()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginAdmin(string adminEmail, string password)
        {
            if (string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == adminEmail);

            if (admin == null)
            {
                ViewBag.ErrorMessage = "Email Admin không tồn tại.";
                return View();
            }

            if (admin.PasswordHash != password)
            {
                ViewBag.ErrorMessage = "Mật khẩu không đúng.";
                return View();
            }

            HttpContext.Session.SetInt32("AdminId", admin.AdminId);
            HttpContext.Session.SetString("AdminName", admin.FullName);

            return RedirectToAction("Dashboard", "Admin");
        }


        // Trang đăng ký cho Sinh viên
        [HttpGet]
        public IActionResult RegisterStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(string fullName, string studentCode, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
            if (existingStudent != null)
            {
                ViewBag.ErrorMessage = "Mã sinh viên đã tồn tại. Vui lòng chọn mã khác.";
                return View();
            }

            var student = new Student
            {
                FullName = fullName,
                StudentCode = studentCode,
                PasswordHash = password,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("LoginStudent","Home");
        }

        // Trang đăng ký cho Công ty
        [HttpGet]
        public IActionResult RegisterCompany()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCompany(string companyName, string companyEmail, string companyPhone, string companyWebsite, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(companyEmail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Email == companyEmail);
            if (existingCompany != null)
            {
                ViewBag.ErrorMessage = "Email công ty đã tồn tại.";
                return View();
            }

            var company = new Company
            {
                Name = companyName,
                Email = companyEmail,
                Phone = companyPhone,
                Website = companyWebsite,
                PasswordHash = password,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký công ty thành công!";
            return RedirectToAction("LoginCompany","Home");
        }

        // Trang đăng ký cho Admin
        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(string adminFullName, string adminEmail, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(adminFullName) || string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == adminEmail);
            if (existingAdmin != null)
            {
                ViewBag.ErrorMessage = "Email Admin đã tồn tại.";
                return View();
            }

            var admin = new Admin
            {
                FullName = adminFullName,
                Email = adminEmail,
                PasswordHash = password,
                CreatedAt = DateTime.Now
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký Admin thành công!";
            return RedirectToAction("LoginAdmin","Home");
        }



        public async Task<IActionResult> Details(int id)
        {
            var jobPosting = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.JobType)
                .Include(j => j.Level)
                .Include(j => j.Skills)
                .FirstOrDefaultAsync(j => j.JobId == id);

            if (jobPosting == null)
            {
                return NotFound();
            }

            return View(jobPosting); 
        }

        public async Task<IActionResult> Profile()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null)
            {
                return RedirectToAction("Login");
            }

            var student = await _context.Students
                .Include(s => s.Skills)
                .FirstOrDefaultAsync(s => s.StudentId == studentId);
            if (student == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.Skills = await _context.Skills.ToListAsync();
            return View(student);
        }


        [HttpPost]
        public async Task<IActionResult> UpdateProfile(int StudentId, IFormFile avatarFile, DateTime? DateOfBirth, string Phone, decimal? GPA, int? GraduationYear, string GitHubProfile, string[] SelectedSkillIds)
        {
            var student = await _context.Students
                .Include(s => s.Skills)
                .FirstOrDefaultAsync(s => s.StudentId == StudentId);
            if (student == null)
            {
                ViewBag.ErrorMessage = "Sinh viên không tồn tại.";
                return View("Profile", student);
            }

            student.DateOfBirth = DateOfBirth.HasValue ? DateOnly.FromDateTime(DateOfBirth.Value) : null;
            student.Phone = Phone;
            student.Gpa = GPA;
            student.GraduationYear = GraduationYear;
            student.GitHubProfile = GitHubProfile;
            student.UpdatedAt = DateTime.Now;

            if (avatarFile != null && avatarFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "images/avatars");
                Directory.CreateDirectory(uploadsFolder);
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                var filePath = Path.Combine(uploadsFolder, fileName);
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }
                student.AvatarUrl = $"/images/avatars/{fileName}";
            }
            _context.Update(student);
            await _context.SaveChangesAsync();
            ViewBag.Skills = await _context.Skills.ToListAsync();
            return View("Profile", student);
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}