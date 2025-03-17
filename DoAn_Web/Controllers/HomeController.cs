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
                .Where(j => j.IsActive == true)
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

            // Tính số thông báo chưa đọc
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
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string role, string username, string password)
        {
            if (string.IsNullOrEmpty(role) || string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Vui lòng điền đầy đủ thông tin.";
                return View();
            }

            if (role == "student")
            {
                var student = await _context.Students
                    .FirstOrDefaultAsync(s => s.StudentCode == username && s.PasswordHash == password);
                if (student != null)
                {
                    HttpContext.Session.SetInt32("StudentId", student.StudentId);
                    HttpContext.Session.SetString("StudentName", student.FullName);
                    return RedirectToAction("Index");
                }
                else
                {
                    ViewBag.ErrorMessage = "Mã sinh viên hoặc mật khẩu không đúng.";
                    return View();
                }
            }
            else if (role == "company")
            {
                var company = await _context.Companies
                    .FirstOrDefaultAsync(c => c.Email == username && c.PasswordHash == password);
                if (company != null)
                {
                    HttpContext.Session.SetInt32("CompanyId", company.CompanyId);
                    HttpContext.Session.SetString("CompanyName", company.Name);
                    return RedirectToAction("CreateJob", "Jobs");
                }
                else
                {
                    ViewBag.ErrorMessage = "Email hoặc mật khẩu không đúng.";
                    return View();
                }
            }

            ViewBag.ErrorMessage = "Vai trò không hợp lệ.";
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(string studentCode, string fullName, string password, string confirmPassword)
        {
            if (string.IsNullOrEmpty(studentCode) || string.IsNullOrEmpty(fullName) || string.IsNullOrEmpty(password))
            {
                ViewBag.ErrorMessage = "Vui lòng nhập đầy đủ thông tin.";
                return View();
            }

            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Mật khẩu nhập lại không khớp.";
                return View();
            }

            var existingStudent = await _context.Students
                .FirstOrDefaultAsync(s => s.StudentCode == studentCode);
            if (existingStudent != null)
            {
                ViewBag.ErrorMessage = "Mã sinh viên đã tồn tại. Vui lòng chọn mã khác.";
                return View();
            }

            var student = new Student
            {
                StudentCode = studentCode,
                FullName = fullName,
                PasswordHash = password,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
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

            //try
            //{
            //    // Xóa các kỹ năng cũ
            //    var existingSkills = await _context.StudentSkills // Sửa "studentSkills" thành "StudentSkills"
            //        .Where(ss => ss.StudentID == StudentId)
            //        .ToListAsync();
            //    _context.StudentSkills.RemoveRange(existingSkills); // Sửa "studentSkills" thành "StudentSkills"
            //    await _context.SaveChangesAsync();

            //    // Thêm các kỹ năng mới từ checkbox
            //    if (SelectedSkillIds != null && SelectedSkillIds.Length > 0)
            //    {
            //        // Log giá trị đầu vào từ checkbox
            //        System.Diagnostics.Debug.WriteLine("SelectedSkillIds from Checkbox: " + string.Join(", ", SelectedSkillIds));

            //        // Chuyển đổi và kiểm tra giá trị hợp lệ
            //        var uniqueSkillIds = SelectedSkillIds
            //            .Where(s => !string.IsNullOrEmpty(s))
            //            .Select(s => int.TryParse(s, out int skillId) ? skillId : (int?)null)
            //            .Where(s => s.HasValue)
            //            .Select(s => s.Value)
            //            .Distinct()
            //            .ToList();

            //        var validSkillIds = await _context.Skills
            //            .Where(s => uniqueSkillIds.Contains(s.SkillId))
            //            .Select(s => s.SkillId)
            //            .ToListAsync();

            //        if (validSkillIds.Count == 0)
            //        {
            //            ViewBag.ErrorMessage = "Không có kỹ năng hợp lệ được chọn.";
            //            ViewBag.Skills = await _context.Skills.ToListAsync();
            //            return View("Profile", student);
            //        }

            //        // Log danh sách kỹ năng hợp lệ
            //        System.Diagnostics.Debug.WriteLine("ValidSkillIds: " + string.Join(", ", validSkillIds));

            //        // Thêm từng kỹ năng một
            //        foreach (var skillId in validSkillIds)
            //        {
            //            var studentSkill = new StudentSkill
            //            {
            //                StudentID = StudentId,
            //                SkillID = skillId
            //            };
            //            _context.StudentSkills.Add(studentSkill); // Sửa "studentSkills" thành "StudentSkills"
            //            await _context.SaveChangesAsync();
            //        }
            //    }

            //    ViewBag.SuccessMessage = "Cập nhật thông tin thành công!";
            //}
            //catch (DbUpdateException ex)
            //{
            //    var innerException = ex.InnerException != null ? ex.InnerException.Message : ex.Message;
            //    ViewBag.ErrorMessage = $"Có lỗi xảy ra khi cập nhật kỹ năng: {innerException}";
            //    System.Diagnostics.Debug.WriteLine("DbUpdateException: " + ex.ToString());
            //}
            //catch (Exception ex)
            //{
            //    ViewBag.ErrorMessage = $"Có lỗi xảy ra khi cập nhật kỹ năng: {ex.Message}";
            //    System.Diagnostics.Debug.WriteLine("Exception: " + ex.ToString());
            //}

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