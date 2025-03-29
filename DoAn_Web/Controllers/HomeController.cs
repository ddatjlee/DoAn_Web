using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

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
        public async Task<IActionResult> Interviews()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem lịch phỏng vấn.";
                return RedirectToAction("LoginStudent");
            }

            var interviews = await _context.Interviews
                .Include(i => i.Application)
                .ThenInclude(a => a.Job)
                .ThenInclude(j => j.Company)
                .Where(i => i.Application.StudentId == studentId.Value)
                .OrderBy(i => i.StartTime)
                .ToListAsync();

            return View(interviews);
        }
        public async Task<IActionResult> Index(int page = 1)
        {
            int pageSize = 3; // Số lượng công việc hiển thị mỗi trang
            var totalJobs = await _context.JobPostings
                .Where(j => j.IsActive == true && j.IsApproved == true)
                .CountAsync(); // Đếm tổng số công việc

            var jobs = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Where(j => j.IsActive == true && j.IsApproved == true)
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)  // Bỏ qua các công việc đã hiển thị
                .Take(pageSize)  // Lấy 4 công việc
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

            // Tính tổng số trang
            var totalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = totalPages;

            return View(model);
        }

        public async Task<IActionResult> GetJobs(int page = 1)
        {
            int pageSize = 3; // Số lượng công việc mỗi trang
            var jobs = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Where(j => j.IsActive == true && j.IsApproved == true)
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return PartialView("_JobList", jobs);
        }

        [HttpGet]
        public async Task<IActionResult> SearchJobs(string keyword, int page = 1)
        {
            int pageSize = 3; // Số công việc mỗi trang

            // Truy vấn cơ bản: lấy các công việc đang hoạt động và đã được duyệt
            var query = _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.Location)
                .Include(j => j.JobType)
                .Include(j => j.Skills)
                .Where(j => j.IsActive == true && j.IsApproved == true);

            // Nếu có từ khóa, tìm kiếm trên nhiều trường
            if (!string.IsNullOrEmpty(keyword))
            {
                keyword = keyword.Trim().ToLower();
                query = query.Where(j =>
                    // Tìm theo tên công ty
                    j.Company.Name.ToLower().Contains(keyword) ||
                    // Tìm theo thành phố
                    (j.Location != null && j.Location.City != null && j.Location.City.ToLower().Contains(keyword)) ||
                    // Tìm theo thể loại công việc
                    (j.JobType != null && j.JobType.Name != null && j.JobType.Name.ToLower().Contains(keyword)) ||
                    // Tìm theo kỹ năng
                    j.Skills.Any(s => s.Name != null && s.Name.ToLower().Contains(keyword))
                );
            }

            // Đếm tổng số công việc khớp với tiêu chí
            var totalJobs = await query.CountAsync();

            // Phân trang
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Lấy danh sách công ty để hiển thị trong phần "Công ty nổi bật"
            var companies = await _context.Companies
                .Where(c => c.Verified == true)
                .OrderByDescending(c => c.CreatedAt)
                .Take(4)
                .ToListAsync();

            // Tạo model để truyền vào view
            var model = new HomeViewModel
            {
                Jobs = jobs,
                Companies = companies
            };

            // Xử lý thông báo chưa đọc và trạng thái đăng nhập (giống như trong Index)
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

            // Truyền thông tin vào ViewBag
            ViewBag.Keyword = keyword;
            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling(totalJobs / (double)pageSize);

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
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            var student = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);

            if (student == null)
            {
                TempData["ErrorMessage"] = "Mật khẩu hoặc tài khoản không đúng.";
                return View();
            }

            if (student.IsLocked == true)
            {
                TempData["ErrorMessage"] = "Tài khoản đã bị khóa!";
                return View();
            }

            if (student.PasswordHash != password)
            {
                TempData["ErrorMessage"] = "Mật khẩu hoặc tài khoản không đúng.";
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
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.Email == companyEmail);

            if (company == null)
            {
                TempData["ErrorMessage"] = "Mật khẩu hoặc Email không đúng.";
                return View();
            }
            if (company.IsLocked == true)
            {
                TempData["ErrorMessage"] = "Tài khoản đã bị khóa!";
                return View();
            }
            if (company.PasswordHash != password)
            {
                TempData["ErrorMessage"] = "Mật khẩu hoặc Email không đúng.";
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
                TempData["ErrorMessage"] = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }
            
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Email == adminEmail);

            if (admin == null)
            {
                TempData["ErrorMessage"] = "Mật khẩu hoặc Email không đúng.";
                return View();
            }
            if (admin.PasswordHash != password)
            {
                TempData["ErrorMessage"] = "Mật khẩu hoặc Email không đúng.";
                return View();
            }

            HttpContext.Session.SetInt32("AdminId", admin.AdminId);
            HttpContext.Session.SetString("AdminName", admin.FullName);

            return RedirectToAction("Index", "Home");
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
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Kiểm tra mã số sinh viên: 10 chữ số
            if (!Regex.IsMatch(studentCode, @"^\d{10}$"))
            {
                TempData["ErrorMessage"] = "Mã số sinh viên phải chứa đúng 10 chữ số.";
                return View();
            }

            // Kiểm tra mật khẩu: ít nhất 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                TempData["ErrorMessage"] = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
                return View();
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            // Kiểm tra mã số sinh viên đã tồn tại
            var existingStudent = await _context.Students.FirstOrDefaultAsync(s => s.StudentCode == studentCode);
            if (existingStudent != null)
            {
                TempData["ErrorMessage"] = "Mã sinh viên đã tồn tại. Vui lòng chọn mã khác.";
                return View();
            }

            // Tạo sinh viên mới
            var student = new Student
            {
                FullName = fullName,
                StudentCode = studentCode,
                PasswordHash = password, // Nên mã hóa mật khẩu trước khi lưu (xem phần lưu ý)
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("LoginStudent", "Home");
        }

        [HttpGet]
        public IActionResult RegisterCompany()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterCompany(string companyName, string companyTaxCode, string companyEmail, string companyPhone, string companyWebsite, string password, string confirmPassword)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(companyName) || string.IsNullOrEmpty(companyTaxCode) || string.IsNullOrEmpty(companyEmail) || string.IsNullOrEmpty(companyPhone) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Kiểm tra email: phải có đuôi @gmail.com
            if (!Regex.IsMatch(companyEmail, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                TempData["ErrorMessage"] = "Email công ty phải có đuôi @gmail.com.";
                return View();
            }

            // Kiểm tra số điện thoại: 10 chữ số
            if (!Regex.IsMatch(companyPhone, @"^\d{10}$"))
            {
                TempData["ErrorMessage"] = "Số điện thoại phải chứa đúng 10 chữ số.";
                return View();
            }

            // Kiểm tra mã số thuế: 10-13 chữ số
            if (!Regex.IsMatch(companyTaxCode, @"^\d{10,13}$"))
            {
                TempData["ErrorMessage"] = "Mã số thuế phải chứa từ 10 đến 13 chữ số.";
                return View();
            }

            // Kiểm tra website: bắt đầu bằng http:// hoặc https://
            if (!string.IsNullOrEmpty(companyWebsite) && !Regex.IsMatch(companyWebsite, @"^https?://"))
            {
                TempData["ErrorMessage"] = "Website phải bắt đầu bằng http:// hoặc https://.";
                return View();
            }

            // Kiểm tra mật khẩu: ít nhất 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                TempData["ErrorMessage"] = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
                return View();
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            // Kiểm tra email công ty đã tồn tại
            var existingCompany = await _context.Companies.FirstOrDefaultAsync(c => c.Email == companyEmail);
            if (existingCompany != null)
            {
                TempData["ErrorMessage"] = "Email công ty đã tồn tại.";
                return View();
            }

            // Tạo công ty mới
            var company = new Company
            {
                Name = companyName,
                TaxCode = companyTaxCode,
                Email = companyEmail,
                Phone = companyPhone,
                Website = companyWebsite,
                PasswordHash = password, // Nên mã hóa mật khẩu trước khi lưu (xem phần lưu ý)
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Companies.Add(company);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký công ty thành công!";
            return RedirectToAction("LoginCompany", "Home");
        }

        [HttpGet]
        public IActionResult RegisterAdmin()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterAdmin(string adminFullName, string adminEmail, string password, string confirmPassword)
        {
            // Kiểm tra các trường bắt buộc
            if (string.IsNullOrEmpty(adminFullName) || string.IsNullOrEmpty(adminEmail) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            // Kiểm tra email: phải có đuôi @gmail.com
            if (!Regex.IsMatch(adminEmail, @"^[a-zA-Z0-9._%+-]+@gmail\.com$"))
            {
                TempData["ErrorMessage"] = "Email admin phải có đuôi @gmail.com.";
                return View();
            }

            // Kiểm tra mật khẩu: ít nhất 8 ký tự, có chữ hoa, chữ thường, số và ký tự đặc biệt
            if (!Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&])[A-Za-z\d@$!%*?&]{8,}$"))
            {
                TempData["ErrorMessage"] = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt.";
                return View();
            }

            // Kiểm tra mật khẩu và xác nhận mật khẩu
            if (password != confirmPassword)
            {
                TempData["ErrorMessage"] = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            // Kiểm tra email admin đã tồn tại
            var existingAdmin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == adminEmail);
            if (existingAdmin != null)
            {
                TempData["ErrorMessage"] = "Email Admin đã tồn tại.";
                return View();
            }

            // Tạo admin mới
            var admin = new Admin
            {
                FullName = adminFullName,
                Email = adminEmail,
                PasswordHash = password, // Nên mã hóa mật khẩu trước khi lưu (xem phần lưu ý)
                CreatedAt = DateTime.Now
            };

            _context.Admins.Add(admin);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký Admin thành công!";
            return RedirectToAction("LoginAdmin", "Home");
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
                TempData["ErrorMessage"] = "Sinh viên không tồn tại.";
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