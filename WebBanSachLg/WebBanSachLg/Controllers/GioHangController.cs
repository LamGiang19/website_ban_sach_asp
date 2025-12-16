using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;
using WebBanSachLg.Helpers;
using WebBanSachLg.Models;

namespace WebBanSachLg.Controllers
{
    public class GioHangController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<GioHangController> _logger;

        public GioHangController(WebBanSachDbContext context, ILogger<GioHangController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private int? GetUserId()
        {
            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                return null;
            }
            return userId;
        }

        private async Task<GioHang> GetOrCreateGioHang(int userId)
        {
            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    TaiKhoanId = userId,
                    NgayTao = DateTime.Now
                };
                _context.GioHangs.Add(gioHang);
                await _context.SaveChangesAsync();
            }

            return gioHang;
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int sachId, int soLuong = 1)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập để thêm sách vào giỏ hàng" });
            }

            var sach = await _context.Saches.FindAsync(sachId);
            if (sach == null || sach.TrangThai != true)
            {
                return Json(new { success = false, message = "Sách không tồn tại hoặc đã ngừng bán" });
            }

            if (soLuong > sach.SoLuong)
            {
                return Json(new { success = false, message = $"Số lượng không đủ. Chỉ còn {sach.SoLuong} cuốn" });
            }

            var gioHang = await GetOrCreateGioHang(userId.Value);

            var chiTietGioHang = await _context.ChiTietGioHangs
                .FirstOrDefaultAsync(c => c.GioHangId == gioHang.Id && c.SachId == sachId);

            if (chiTietGioHang != null)
            {
                var soLuongMoi = chiTietGioHang.SoLuong + soLuong;
                if (soLuongMoi > sach.SoLuong)
                {
                    return Json(new { success = false, message = $"Số lượng không đủ. Chỉ còn {sach.SoLuong} cuốn" });
                }
                chiTietGioHang.SoLuong = soLuongMoi;
                chiTietGioHang.Gia = sach.Gia;
            }
            else
            {
                chiTietGioHang = new ChiTietGioHang
                {
                    GioHangId = gioHang.Id,
                    SachId = sachId,
                    SoLuong = soLuong,
                    Gia = sach.Gia,
                    NgayThem = DateTime.Now
                };
                _context.ChiTietGioHangs.Add(chiTietGioHang);
            }

            await _context.SaveChangesAsync();

            var tongSoLuong = await _context.ChiTietGioHangs
                .Where(c => c.GioHangId == gioHang.Id)
                .SumAsync(c => c.SoLuong);

            return Json(new { success = true, message = "Đã thêm vào giỏ hàng", tongSoLuong });
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem giỏ hàng";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);

            var items = new List<GioHangViewModel>();

            if (gioHang != null)
            {
                var chiTietGioHangs = await _context.ChiTietGioHangs
                    .Where(c => c.GioHangId == gioHang.Id)
                    .Include(c => c.Sach)
                    .ToListAsync();

                items = chiTietGioHangs.Select(c => new GioHangViewModel
                {
                    Id = c.Id,
                    SachId = c.SachId,
                    TenSach = c.Sach.TenSach,
                    HinhAnh = c.Sach.HinhAnh,
                    Gia = c.Gia,
                    SoLuong = c.SoLuong,
                    SoLuongTon = c.Sach.SoLuong
                }).ToList();
            }

            var model = new GioHangIndexViewModel
            {
                Items = items
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateQuantity(int chiTietGioHangId, int soLuong)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var chiTietGioHang = await _context.ChiTietGioHangs
                .Include(c => c.Sach)
                .Include(c => c.GioHang)
                .FirstOrDefaultAsync(c => c.Id == chiTietGioHangId && c.GioHang.TaiKhoanId == userId.Value);

            if (chiTietGioHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }

            if (soLuong <= 0)
            {
                return Json(new { success = false, message = "Số lượng phải lớn hơn 0" });
            }

            if (soLuong > chiTietGioHang.Sach.SoLuong)
            {
                return Json(new { success = false, message = $"Số lượng không đủ. Chỉ còn {chiTietGioHang.Sach.SoLuong} cuốn" });
            }

            chiTietGioHang.SoLuong = soLuong;
            await _context.SaveChangesAsync();

            var tongTien = chiTietGioHang.Gia * soLuong;
            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);
            
            var tongTienGioHang = 0m;
            var tongSoLuong = 0;
            if (gioHang != null)
            {
                tongTienGioHang = await _context.ChiTietGioHangs
                    .Where(c => c.GioHangId == gioHang.Id)
                    .SumAsync(c => c.Gia * c.SoLuong);
                
                tongSoLuong = await _context.ChiTietGioHangs
                    .Where(c => c.GioHangId == gioHang.Id)
                    .SumAsync(c => c.SoLuong);
            }

            return Json(new { 
                success = true, 
                tongTien = tongTien,
                tongTienGioHang = tongTienGioHang,
                tongSoLuong = tongSoLuong
            });
        }

        [HttpPost]
        public async Task<IActionResult> Remove(int chiTietGioHangId)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var chiTietGioHang = await _context.ChiTietGioHangs
                .Include(c => c.GioHang)
                .FirstOrDefaultAsync(c => c.Id == chiTietGioHangId && c.GioHang.TaiKhoanId == userId.Value);

            if (chiTietGioHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }

            _context.ChiTietGioHangs.Remove(chiTietGioHang);
            await _context.SaveChangesAsync();

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);
            
            var tongTienGioHang = 0m;
            var tongSoLuong = 0;
            if (gioHang != null)
            {
                tongTienGioHang = await _context.ChiTietGioHangs
                    .Where(c => c.GioHangId == gioHang.Id)
                    .SumAsync(c => c.Gia * c.SoLuong);
                
                tongSoLuong = await _context.ChiTietGioHangs
                    .Where(c => c.GioHangId == gioHang.Id)
                    .SumAsync(c => c.SoLuong);
            }

            return Json(new { 
                success = true, 
                message = "Đã xóa sản phẩm khỏi giỏ hàng",
                tongTienGioHang = tongTienGioHang,
                tongSoLuong = tongSoLuong
            });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartCount()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { count = 0 });
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);

            if (gioHang == null)
            {
                return Json(new { count = 0 });
            }

            var count = await _context.ChiTietGioHangs
                .Where(c => c.GioHangId == gioHang.Id)
                .SumAsync(c => c.SoLuong);

            return Json(new { count });
        }

        [HttpGet]
        public async Task<IActionResult> GetCartItems()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { items = new List<object>(), tongTien = 0 });
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);

            if (gioHang == null)
            {
                return Json(new { items = new List<object>(), tongTien = 0 });
            }

            var chiTietGioHangs = await _context.ChiTietGioHangs
                .Where(c => c.GioHangId == gioHang.Id)
                .Include(c => c.Sach)
                .ToListAsync();

            var items = chiTietGioHangs.Select(c => new
            {
                id = c.Id,
                sachId = c.SachId,
                tenSach = c.Sach.TenSach,
                hinhAnh = FileUploadHelper.GetSachImagePath(c.Sach.HinhAnh),
                gia = c.Gia,
                soLuong = c.SoLuong,
                thanhTien = c.Gia * c.SoLuong
            }).ToList();

            var tongTien = items.Sum(i => i.thanhTien);

            return Json(new { items, tongTien });
        }
    }
}

