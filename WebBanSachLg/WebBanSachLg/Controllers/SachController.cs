using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;
using WebBanSachLg.Helpers;
using WebBanSachLg.Models;

namespace WebBanSachLg.Controllers
{
    public class SachController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<SachController> _logger;

        public SachController(WebBanSachDbContext context, ILogger<SachController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int? danhMucId, int? tacGiaId, decimal? giaMin, decimal? giaMax, string? sapXep, int page = 1, int pageSize = 12)
        {
            var query = _context.Saches
                .Where(s => s.TrangThai == true)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .AsQueryable();

            // Lọc theo danh mục
            if (danhMucId.HasValue)
            {
                query = query.Where(s => s.DanhMucId == danhMucId.Value);
            }

            // Lọc theo tác giả
            if (tacGiaId.HasValue)
            {
                query = query.Where(s => s.TacGiaId == tacGiaId.Value);
            }

            // Lọc theo giá
            if (giaMin.HasValue)
            {
                query = query.Where(s => s.Gia >= giaMin.Value);
            }
            if (giaMax.HasValue)
            {
                query = query.Where(s => s.Gia <= giaMax.Value);
            }

            // Sắp xếp
            query = sapXep switch
            {
                "gia-tang" => query.OrderBy(s => s.Gia),
                "gia-giam" => query.OrderByDescending(s => s.Gia),
                "ten" => query.OrderBy(s => s.TenSach),
                "moi-nhat" => query.OrderByDescending(s => s.NgayTao),
                _ => query.OrderByDescending(s => s.NgayTao)
            };

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var saches = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            ViewBag.DanhMucs = await _context.DanhMucs.Where(d => d.TrangThai == true).ToListAsync();
            ViewBag.TacGias = await _context.TacGia.Where(t => t.TrangThai == true).ToListAsync();
            ViewBag.DanhMucId = danhMucId;
            ViewBag.TacGiaId = tacGiaId;
            ViewBag.GiaMin = giaMin;
            ViewBag.GiaMax = giaMax;
            ViewBag.SapXep = sapXep;
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.TotalPages = totalPages;

            return View(saches);
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var sach = await _context.Saches
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .FirstOrDefaultAsync(s => s.Id == id && s.TrangThai == true);

            if (sach == null)
            {
                return NotFound();
            }

            // Lấy sách cùng danh mục
            var sachCungDanhMuc = await _context.Saches
                .Where(s => s.DanhMucId == sach.DanhMucId && s.Id != sach.Id && s.TrangThai == true)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Take(4)
                .ToListAsync();

            ViewBag.SachCungDanhMuc = sachCungDanhMuc;

            return View(sach);
        }

        public async Task<IActionResult> Search(string? keyword, int? categoryId, int page = 1, int pageSize = 12)
        {
            var query = _context.Saches
                .Where(s => s.TrangThai == true)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .AsQueryable();

            // Tìm kiếm theo từ khóa
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(s => s.TenSach.Contains(keyword) || 
                                        (s.MoTa != null && s.MoTa.Contains(keyword)));
            }

            // Lọc theo danh mục
            if (categoryId.HasValue)
            {
                query = query.Where(s => s.DanhMucId == categoryId.Value);
            }

            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var saches = await query
                .OrderByDescending(s => s.NgayTao)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var model = new TimKiemViewModel
            {
                Keyword = keyword,
                DanhMucId = categoryId,
                Page = page,
                PageSize = pageSize,
                TotalItems = totalItems,
                Saches = saches,
                DanhMucs = await _context.DanhMucs.Where(d => d.TrangThai == true).ToListAsync()
            };

            return View(model);
        }
    }
}

