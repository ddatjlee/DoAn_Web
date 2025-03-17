using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace YourNamespace.Controllers
{
    public class JobsController : Controller
    {
        private readonly RecruitmentSystemContext _context;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public JobsController(RecruitmentSystemContext context, IWebHostEnvironment hostingEnvironment)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
        }

        public async Task<IActionResult> Index(string searchString, int? locationId, int pageNumber = 1)
        {
            int pageSize = 6;

            ViewBag.Locations = await _context.Locations.ToListAsync();

            var jobsQuery = _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.JobType)
                .Include(j => j.Level)
                .Include(j => j.Location)
                .Where(j => j.IsActive == true);

            if (!string.IsNullOrEmpty(searchString))
            {
                jobsQuery = jobsQuery.Where(j => j.Title.Contains(searchString) || j.Company.Name.Contains(searchString));
            }

            if (locationId.HasValue)
            {
                jobsQuery = jobsQuery.Where(j => j.LocationId == locationId);
            }

            var totalJobs = await jobsQuery.CountAsync();

            var jobs = await jobsQuery
                .OrderByDescending(j => j.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

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

        public async Task<IActionResult> Details(int id)
        {
            var job = await _context.JobPostings
                .Include(j => j.Company)
                .Include(j => j.JobType)
                .Include(j => j.Level)
                .Include(j => j.Location)
                .FirstOrDefaultAsync(m => m.JobId == id);

            if (job == null)
            {
                return NotFound();
            }

            return View(job);
        }

        [HttpGet]
        public IActionResult CreateJob()
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (companyId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Login", "Home");
            }

            ViewBag.JobTypes = _context.JobTypes.ToList();
            ViewBag.ExperienceLevels = _context.ExperienceLevels.ToList();
            ViewBag.Locations = _context.Locations.ToList();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateJob(JobPosting jobPosting)
        {
            var companyId = HttpContext.Session.GetInt32("CompanyId");
            if (companyId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập với tư cách công ty.";
                return RedirectToAction("Login", "Home");
            }

            if (ModelState.IsValid)
            {
                jobPosting.CompanyId = companyId.Value;
                jobPosting.CreatedAt = DateTime.Now;
                jobPosting.UpdatedAt = DateTime.Now;

                _context.JobPostings.Add(jobPosting);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đăng tin tuyển dụng thành công!";
                return RedirectToAction("Index");
            }

            ViewBag.JobTypes = _context.JobTypes.ToList();
            ViewBag.ExperienceLevels = _context.ExperienceLevels.ToList();
            ViewBag.Locations = _context.Locations.ToList();
            return View(jobPosting);
        }
    }
}