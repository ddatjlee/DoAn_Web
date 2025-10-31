using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using DoAn_Web.Models;
using System.Linq;

namespace DoAn_Web.Controllers
{
    public class LegalBasesController : Controller
    {
        private readonly RecruitmentSystemContext _context;

        public LegalBasesController(RecruitmentSystemContext context)
        {
            _context = context;
        }

        // GET: /LegalBases
        public async Task<IActionResult> Index()
        {
            // Order by LegalID as CreatedAt may not exist in the target DB schema
            var items = await _context.LegalBases
                .AsNoTracking()
                .OrderByDescending(lb => lb.LegalID)
                .ToListAsync();
            return View(items);
        }
    }
}
