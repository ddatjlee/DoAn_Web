using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using DoAn_Web.Models;

namespace YourNamespace.Controllers
{
    public class JobsController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public JobsController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách việc làm với lọc và phân trang
        public async Task<IActionResult> Index(string searchString, int? locationId, int pageNumber = 1)
        {
            int pageSize = 6;

            // Lấy danh sách địa điểm để hiển thị trong dropdown lọc
            ViewBag.Locations = await _context.Locations.ToListAsync();

            var jobsQuery = _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.JobType)
                .Include(j => j.Level)
                .Include(j => j.Location) // Include Location thay vì Locations
                .Include(j => j.Skills)   // Include Skills
                .Where(j => j.IsActive == true); // Chỉ lấy các công việc đang hoạt động

            // Lọc theo từ khóa tìm kiếm
            if (!string.IsNullOrEmpty(searchString))
            {
                jobsQuery = jobsQuery.Where(j => j.Title.Contains(searchString) || j.Company.Name.Contains(searchString));
            }

            // Lọc theo địa điểm
            if (locationId.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.LocationId == locationId);
            }

            // Đếm tổng số công việc để phân trang
            var totalJobs = await jobsQuery.CountAsync();

            // Phân trang
            var jobs = await jobsQuery
                .OrderByDescending(j => j.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Tạo model để truyền vào view
            var model = new
            {
                Jobs = jobs,
                TotalPages = (int)Math.Ceiling(totalJobs / (double)pageSize),
                CurrentPage = pageNumber,
                SearchString = searchString,
                LocationId = locationId
            };

            return View(model);
        }

        // Hiển thị chi tiết công việc
        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.JobType)
                .Include(j => j.Level)
                .Include(j => j.Location) // Include Location thay vì Locations
                .Include(j => j.Skills)   // Include Skills
                .FirstOrDefaultAsync(m => m.JobId == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }
    }
}