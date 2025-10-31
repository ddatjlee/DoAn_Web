using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace DoAn_Web.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public SupervisorController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Danh sách sinh viên được hướng dẫn
        public async Task<IActionResult> StudentList()
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internships = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.Company)
                .Include(i => i.WeeklyReports)
                .Include(i => i.SupervisorEvaluation)
                .Where(i => i.SupervisorId == supervisorId.Value)
                .ToListAsync();

            return View(internships);
        }

        // Xem danh sách báo cáo của sinh viên
        public async Task<IActionResult> ViewStudentReports(int studentId)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                return RedirectToAction("LoginSupervisor", "Home");
            }

            // Kiểm tra quyền truy cập
            var internship = await _context.Internships
                .Include(i => i.Student)
                .FirstOrDefaultAsync(i => i.StudentId == studentId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền xem báo cáo của sinh viên này hoặc sinh viên không thuộc danh sách hướng dẫn của bạn.";
                return RedirectToAction("StudentList");
            }

            var reports = await _context.WeeklyReports
                .Where(r => r.InternshipId == internship.InternshipId)
                .OrderByDescending(r => r.WeekNumber)
                .ToListAsync();

            ViewBag.StudentName = internship.Student.FullName;
            return View(reports);
        }

        // Xem và duyệt báo cáo
        public async Task<IActionResult> ReviewReport(int reportId)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để duyệt báo cáo.";
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var report = await _context.WeeklyReports
                .Include(r => r.Internship)
                    .ThenInclude(i => i.Student)
                .Include(r => r.Internship)
                    .ThenInclude(i => i.Company)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy báo cáo này trong hệ thống.";
                return RedirectToAction("StudentList");
            }
            
            if (report.Internship.SupervisorId != supervisorId.Value)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền duyệt báo cáo này vì sinh viên không thuộc danh sách hướng dẫn của bạn.";
                return RedirectToAction("StudentList");
            }

            return View(report);
        }

        [HttpPost]
        public async Task<IActionResult> ReviewReport(int reportId, string decision, string comment)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var report = await _context.WeeklyReports
                .Include(r => r.Internship)
                .FirstOrDefaultAsync(r => r.ReportId == reportId);

            if (report == null || report.Internship.SupervisorId != supervisorId.Value)
            {
                TempData["ErrorMessage"] = "Không tìm thấy báo cáo hoặc bạn không có quyền duyệt.";
                return RedirectToAction("StudentList");
            }

            report.Status = "approved";
            report.SupervisorComment = comment;
            report.ReviewedAt = DateTime.Now;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã duyệt báo cáo thành công.";
            return RedirectToAction("ViewStudentReports", new { studentId = report.Internship.StudentId });
        }

        // Đánh giá thực tập sinh
        public async Task<IActionResult> EvaluateIntern(int internshipId)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.Company)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập hoặc bạn không có quyền đánh giá.";
                return RedirectToAction("StudentList");
            }

            ViewBag.StudentName = internship.Student.FullName;
            ViewBag.StudentCode = internship.Student.StudentCode;
            ViewBag.CompanyName = internship.Company.Name;
            ViewBag.Duration = $"{internship.StartDate:dd/MM/yyyy} - {internship.EndDate:dd/MM/yyyy}";
            ViewBag.InternshipId = internshipId;

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EvaluateIntern(int internshipId, int score, string comments)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internship = await _context.Internships
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập hoặc bạn không có quyền đánh giá.";
                return RedirectToAction("StudentList");
            }

            var evaluation = new SupervisorEvaluation
            {
                InternshipId = internshipId,
                Score = score,
                Comments = comments,
                EvaluationDate = DateTime.Now
            };

            _context.SupervisorEvaluations.Add(evaluation);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã lưu đánh giá thành công.";
            return RedirectToAction("StudentList");
        }

        // Xem đánh giá của công ty cho sinh viên
        public async Task<IActionResult> ViewCompanyEvaluation(int internshipId)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internship = await _context.Internships
                .Include(i => i.Student)
                .Include(i => i.Company)
                .Include(i => i.CompanyEvaluation)
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy thông tin thực tập hoặc bạn không có quyền xem.";
                return RedirectToAction("StudentList");
            }

            if (internship.CompanyEvaluation == null)
            {
                TempData["InfoMessage"] = "Công ty chưa đánh giá sinh viên này.";
                return RedirectToAction("StudentList");
            }

            return View(internship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkInternshipComplete(int internshipId)
        {
            var supervisorId = HttpContext.Session.GetInt32("SupervisorId");
            if (!supervisorId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để thực hiện hành động.";
                return RedirectToAction("LoginSupervisor", "Home");
            }

            var internship = await _context.Internships
                .FirstOrDefaultAsync(i => i.InternshipId == internshipId && i.SupervisorId == supervisorId.Value);

            if (internship == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy bản ghi thực tập hoặc bạn không có quyền thực hiện.";
                return RedirectToAction("StudentList");
            }

            internship.Status = "Hoàn thành";
            if (!internship.EndDate.HasValue)
            {
                internship.EndDate = DateTime.Now;
            }

            _context.Internships.Update(internship);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã cập nhật trạng thái thực tập: Hoàn thành.";
            return RedirectToAction("StudentList");
        }
    }
}