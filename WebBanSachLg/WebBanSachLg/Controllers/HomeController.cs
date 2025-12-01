using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;
using WebBanSachLg.Models;

namespace WebBanSachLg.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly WebBanSachDbContext _context;

        public HomeController(ILogger<HomeController> logger, WebBanSachDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy danh mục cho sidebar
            var danhMucs = _context.DanhMucs
                .Where(d => d.TrangThai == true)
                .ToList();
            ViewBag.DanhMucs = danhMucs;

            // Lấy sách bán chạy (có thể lấy theo số lượng đơn hàng hoặc random)
            var sachBanChay = _context.Saches
                .Where(s => s.TrangThai == true)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .OrderByDescending(s => s.NgayTao)
                .Take(8)
                .ToList();

            // Lấy sách theo danh mục
            var sachTieuThuyet = _context.Saches
                .Where(s => s.TrangThai == true && s.DanhMucId == 1)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Take(8)
                .ToList();

            var sachKhoaHoc = _context.Saches
                .Where(s => s.TrangThai == true && s.DanhMucId == 2)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Take(8)
                .ToList();

            var sachThieuNhi = _context.Saches
                .Where(s => s.TrangThai == true && s.DanhMucId == 5)
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Take(8)
                .ToList();

            ViewBag.SachBanChay = sachBanChay;
            ViewBag.SachTieuThuyet = sachTieuThuyet;
            ViewBag.SachKhoaHoc = sachKhoaHoc;
            ViewBag.SachThieuNhi = sachThieuNhi;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
