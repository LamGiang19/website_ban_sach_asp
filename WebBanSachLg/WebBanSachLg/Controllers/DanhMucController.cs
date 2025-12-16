using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;

namespace WebBanSachLg.Controllers
{
    public class DanhMucController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<DanhMucController> _logger;

        public DanhMucController(WebBanSachDbContext context, ILogger<DanhMucController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var danhMucs = await _context.DanhMucs
                .Where(d => d.TrangThai == true)
                .OrderBy(d => d.TenDanhMuc)
                .ToListAsync();

            return View(danhMucs);
        }

        public async Task<IActionResult> Details(int? id, int page = 1, int pageSize = 12)
        {
            if (id == null)
            {
                return NotFound();
            }

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null || danhMuc.TrangThai != true)
            {
                return NotFound();
            }

            var query = _context.Saches
                .Where(s => s.DanhMucId == id && s.TrangThai == true)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .OrderByDescending(s => s.NgayTao);

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var saches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.DanhMuc = danhMuc;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return View(saches);
        }
    }
}

