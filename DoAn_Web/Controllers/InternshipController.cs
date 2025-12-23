using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace DoAn_Web.Controllers
{
    public class InternshipController : Controller
    {
        private readonly RecruitmentSystemContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public InternshipController(RecruitmentSystemContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> StudentReports()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");

            if (!studentId.HasValue && !supervisorId.HasValue)
            {
                return RedirectToAction("Index", "Home");
            }

            IQueryable<Internship> query = _context.Internships;

            // Áp dụng điều kiện trước
            if (studentId.HasValue)
            {
                query = query.Where(i => i.StudentId == studentId.Value);
            }
            else if (supervisorId.HasValue)
            {
                query = query.Where(i => i.SupervisorId == supervisorId.Value);
            }

            // Sau đó mới Include các related entities
            query = query
                .Include(i => i.WeeklyReports)
                .Include(i => i.Company)
                .Include(i => i.Supervisor)
                .Include(i => i.Student);

            var internships = await query.ToListAsync();

            if (!internships.Any())
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            // Nếu là sinh viên, chỉ trả về báo cáo của sinh viên đó
            if (studentId.HasValue)
            {
                return View(internships.First());
            }
            
            // Nếu là giảng viên, trả về list tất cả báo cáo của sinh viên được hướng dẫn
            return View("SupervisorStudentReports", internships);
        }

        [HttpGet]
        public async Task<IActionResult> CreateReport()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tạo báo cáo.";
                return RedirectToAction("LoginStudent", "Home");
            }

            var internship = await _context.Internships
                .FirstOrDefaultAsync(i => i.StudentId == studentId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateReport(string reportContent)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tạo báo cáo.";
                return RedirectToAction("LoginStudent", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.WeeklyReports)
                .FirstOrDefaultAsync(i => i.StudentId == studentId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            var currentWeek = (int)((DateTime.Now - internship.StartDate).TotalDays / 7) + 1;

            var report = new WeeklyReport
            {
                InternshipId = internship.InternshipId,
                WeekNumber = currentWeek,
                ReportDate = DateTime.Now,
                Content = reportContent
            };

            _context.WeeklyReports.Add(report);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Báo cáo tuần " + currentWeek + " đã được gửi thành công!";
            return RedirectToAction("StudentReports");
        }

        [HttpGet]
        public async Task<IActionResult> CompanyEvaluation(int internshipId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đánh giá.";
                return RedirectToAction("LoginCompany", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.CompanyEvaluation)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.CompanyId == companyId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            return View(internship);
        }

        [HttpPost]
        public async Task<IActionResult> CompanyEvaluation(int internshipId, decimal criteriaCompliance, decimal criteriaTaskPerformance, decimal criteriaRelationship, string comments)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đánh giá.";
                return RedirectToAction("LoginCompany", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.CompanyEvaluation)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.CompanyId == companyId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            // Tính điểm tổng kết (trung bình cộng của 3 tiêu chí, quy về thang 10)
            var averageScore = (criteriaCompliance + criteriaTaskPerformance + criteriaRelationship) / 3;
            var finalScore = (int)Math.Round(averageScore); // Làm tròn thành số nguyên

            if (internship.CompanyEvaluation == null)
            {
                var evaluation = new CompanyEvaluation
                {
                    InternshipId = internshipId,
                    CriteriaCompliance = criteriaCompliance,
                    CriteriaTaskPerformance = criteriaTaskPerformance,
                    CriteriaRelationship = criteriaRelationship,
                    Score = finalScore,
                    Comments = comments,
                    EvaluationDate = DateTime.Now
                };
                _context.CompanyEvaluations.Add(evaluation);
            }
            else
            {
                internship.CompanyEvaluation.CriteriaCompliance = criteriaCompliance;
                internship.CompanyEvaluation.CriteriaTaskPerformance = criteriaTaskPerformance;
                internship.CompanyEvaluation.CriteriaRelationship = criteriaRelationship;
                internship.CompanyEvaluation.Score = finalScore;
                internship.CompanyEvaluation.Comments = comments;
                internship.CompanyEvaluation.EvaluationDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đánh giá đã được lưu thành công!";
            return RedirectToAction("InternList");
        }

        [HttpGet]
        public async Task<IActionResult> SupervisorEvaluation(int internshipId)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đánh giá.";
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.Company)
                .Include(i => i.WeeklyReports)
                .Include(i => i.CompanyEvaluation)
                .Include(i => i.SupervisorEvaluation)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            return View(internship);
        }

        [HttpPost]
        public async Task<IActionResult> SupervisorEvaluation(int internshipId, int score, string comments)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đánh giá.";
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.SupervisorEvaluation)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập hoặc bạn không có quyền đánh giá sinh viên này.";
                return RedirectToAction("Index", "Home");
            }

            if (internship.SupervisorEvaluation == null)
            {
                var evaluation = new SupervisorEvaluation
                {
                    InternshipId = internshipId,
                    Score = score,
                    Comments = comments,
                    EvaluationDate = DateTime.Now
                };
                _context.SupervisorEvaluations.Add(evaluation);
            }
            else
            {
                internship.SupervisorEvaluation.Score = score;
                internship.SupervisorEvaluation.Comments = comments;
                internship.SupervisorEvaluation.EvaluationDate = DateTime.Now;
            }

            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Đánh giá đã được lưu thành công!";
            return RedirectToAction("SupervisorDashboard");
        }

        public async Task<IActionResult> ViewReports()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem báo cáo.";
                return RedirectToAction("LoginStudent", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.WeeklyReports)
                .FirstOrDefaultAsync(i => i.StudentId == studentId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            return View(internship);
        }

        public async Task<IActionResult> ViewEvaluations()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem đánh giá.";
                return RedirectToAction("LoginStudent", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.CompanyEvaluation)
                .Include(i => i.SupervisorEvaluation)
                .FirstOrDefaultAsync(i => i.StudentId == studentId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("Index", "Home");
            }

            return View(internship);
        }

        public async Task<IActionResult> SupervisorDashboard()
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem dashboard.";
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internships = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.Company)
                .Include(i => i.WeeklyReports)
                .Include(i => i.CompanyEvaluation)
                .Include(i => i.SupervisorEvaluation)
                .Where(i => i.SupervisorId == supervisorId.Value)
                .ToListAsync();

            return View(internships);
        }

        public async Task<IActionResult> InternList()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem danh sách sinh viên thực tập.";
                return RedirectToAction("LoginCompany", "Home");
            }

            var internships = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.WeeklyReports)
                .Include(i => i.CompanyEvaluation)
                .Where(i => i.CompanyId == companyId.Value)
                .ToListAsync();

            return View(internships);
        }

        public async Task<IActionResult> ViewReport(int reportId)
        {
            var report = await _context.WeeklyReports
                .Include(r => r.Internship)
                .ThenInclude(i => i.Student)
                .Include(r => r.Internship)
                .ThenInclude(i => i.Company)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy báo cáo.";
                return RedirectToAction("Index", "Home");
            }

            // Kiểm tra quyền truy cập
            var studentId = HttpContext.Session.GetInt32("StudentId");
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            var companyId = HttpContext.Session.GetInt32("CompanyId");

            if (studentId.HasValue && report.Internship.StudentId != studentId.Value)
            {
                return Forbid();
            }
            else if (supervisorId.HasValue && report.Internship.SupervisorId != supervisorId.Value)
            {
                return Forbid();
            }
            else if (companyId.HasValue && report.Internship.CompanyId != companyId.Value)
            {
                return Forbid();
            }

            return View(report);
        }

        public async Task<IActionResult> WeeklyReportTable()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem báo cáo.";
                return RedirectToAction("LoginStudent", "Home");
            }

            // Tìm bản ghi thực tập đã được phân công
            var internship = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.Company)
                .Include(i => i.Supervisor)
                .Include(i => i.WeeklyReports)
                .FirstOrDefaultAsync(i => i.StudentId == studentId.Value);

            if (internship == null)
            {
                // Kiểm tra xem sinh viên đã được chấp nhận và phân công chưa
                var assignedApplication = await _context.Applications
                    .Include(a => a.Job)
                        .ThenInclude(j => j.Company)
                    .Include(a => a.Student)
                    .FirstOrDefaultAsync(a => a.StudentId == studentId.Value && a.Status == "assigned");

                var pendingApplication = await _context.Applications
                    .Include(a => a.Job)
                        .ThenInclude(j => j.Company)
                    .FirstOrDefaultAsync(a => a.StudentId == studentId.Value && a.Status == "Accepted");

                if (pendingApplication != null)
                {
                    TempData["InfoMessage"] = "Đơn ứng tuyển của bạn đã được chấp nhận. Vui lòng đợi Admin phân công giảng viên hướng dẫn.";
                    return RedirectToAction("Index", "Home");
                }

                TempData["ErrorMessage"] = "Bạn chưa được phân công thực tập hoặc đơn ứng tuyển chưa được chấp nhận.";
                return RedirectToAction("Index", "Home");

                // Load lại dữ liệu sau khi tạo
                internship = await _context.Internships
                    .Include(i => i.Student)
                    .Include(i => i.Company)
                    .Include(i => i.Supervisor)
                    .Include(i => i.WeeklyReports)
                    .FirstOrDefaultAsync(i => i.StudentId == studentId.Value);
            }

            return View(internship);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitReport(int weekNumber, string content, int internshipId)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để nộp báo cáo." });
            }

            var internship = await _context.Internships
                .Include(i => i.WeeklyReports)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.StudentId == studentId.Value);

            if (internship == null)
            {
                return Json(new { success = false, message = "Không tìm thấy thông tin thực tập." });
            }

            // Kiểm tra xem đã có báo cáo tuần này chưa
            if (internship.WeeklyReports.Any(r => r.WeekNumber == weekNumber))
            {
                return Json(new { success = false, message = "Báo cáo tuần này đã được nộp." });
            }

            var report = new WeeklyReport
            {
                InternshipId = internshipId,
                WeekNumber = weekNumber,
                Content = content,
                ReportDate = DateTime.Now,
                Status = "Đã nộp"
            };

            _context.WeeklyReports.Add(report);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        [HttpPost]
        public async Task<IActionResult> UploadInternshipReport(int internshipId, IFormFile reportFile)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (!studentId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để gửi báo cáo.";
                return RedirectToAction("LoginStudent", "Home");
            }

            var internship = await _context.Internships
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.StudentId == studentId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập.";
                return RedirectToAction("WeeklyReportTable");
            }

            if (reportFile == null || reportFile.Length == 0)
            {
                TempData["ErrorMessage"] = "Vui lòng chọn file báo cáo.";
                return RedirectToAction("WeeklyReportTable");
            }

            // Kiểm tra định dạng file
            var allowedExtensions = new[] { ".pdf", ".doc", ".docx" };
            var fileExtension = Path.GetExtension(reportFile.FileName).ToLower();
            if (!allowedExtensions.Contains(fileExtension))
            {
                TempData["ErrorMessage"] = "Chỉ chấp nhận các file: PDF, Word (.doc, .docx)";
                return RedirectToAction("WeeklyReportTable");
            }

            // Kiểm tra kích thước file (max 10MB)
            if (reportFile.Length > 10 * 1024 * 1024)
            {
                TempData["ErrorMessage"] = "Kích thước file không được vượt quá 10MB.";
                return RedirectToAction("WeeklyReportTable");
            }

            try
            {
                var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, "internship-reports");
                Directory.CreateDirectory(uploadsFolder);
                
                var fileName = $"internship_{internshipId}_{DateTime.Now:yyyyMMdd_HHmmss}{fileExtension}";
                var filePath = Path.Combine(uploadsFolder, fileName);
                
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await reportFile.CopyToAsync(stream);
                }

                internship.InternshipReportUrl = $"/internship-reports/{fileName}";
                _context.Internships.Update(internship);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Báo cáo thực tập đã được gửi thành công!";
                return RedirectToAction("WeeklyReportTable");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Có lỗi xảy ra khi gửi báo cáo: {ex.Message}";
                return RedirectToAction("WeeklyReportTable");
            }
        }

        // Company xem báo cáo của sinh viên thực tập
        public async Task<IActionResult> ViewStudentReports(int internshipId)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (!companyId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem báo cáo.";
                return RedirectToAction("LoginCompany", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.WeeklyReports)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.CompanyId == companyId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập hoặc bạn không có quyền xem.";
                return RedirectToAction("InternList");
            }

            ViewBag.StudentName = internship.Student.FullName;
            ViewBag.StudentCode = internship.Student.StudentCode;
            ViewBag.InternshipId = internshipId;
            ViewBag.InternshipReportUrl = internship.InternshipReportUrl;

            var reports = internship.WeeklyReports.OrderByDescending(r => r.WeekNumber).ToList();

            return View(reports);
        }
    }
}