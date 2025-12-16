using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;
using WebBanSachLg.Models;

namespace WebBanSachLg.Controllers
{
    public class DonHangController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<DonHangController> _logger;

        public DonHangController(WebBanSachDbContext context, ILogger<DonHangController> logger)
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

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var gioHang = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);

            if (gioHang == null)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "GioHang");
            }

            var chiTietGioHangs = await _context.ChiTietGioHangs
                .Where(c => c.GioHangId == gioHang.Id)
                .Include(c => c.Sach)
                .ToListAsync();

            if (!chiTietGioHangs.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "GioHang");
            }

            var gioHangItems = chiTietGioHangs.Select(c => new GioHangViewModel
            {
                Id = c.Id,
                SachId = c.SachId,
                TenSach = c.Sach.TenSach,
                HinhAnh = c.Sach.HinhAnh,
                Gia = c.Gia,
                SoLuong = c.SoLuong,
                SoLuongTon = c.Sach.SoLuong
            }).ToList();

            var taiKhoan = await _context.TaiKhoans.FindAsync(userId.Value);

            var model = new DonHangCreateViewModel
            {
                GioHangItems = gioHangItems,
                TongTien = gioHangItems.Sum(i => i.ThanhTien),
                HoTenNguoiNhan = taiKhoan?.HoTen ?? "",
                SoDienThoai = taiKhoan?.SoDienThoai ?? "",
                DiaChiGiaoHang = taiKhoan?.DiaChi ?? ""
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DonHangCreateViewModel model)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để đặt hàng";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!ModelState.IsValid)
            {
                var gioHang = await _context.GioHangs
                    .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);

                if (gioHang != null)
                {
                    var chiTietGioHangs = await _context.ChiTietGioHangs
                        .Where(c => c.GioHangId == gioHang.Id)
                        .Include(c => c.Sach)
                        .ToListAsync();

                    model.GioHangItems = chiTietGioHangs.Select(c => new GioHangViewModel
                    {
                        Id = c.Id,
                        SachId = c.SachId,
                        TenSach = c.Sach.TenSach,
                        HinhAnh = c.Sach.HinhAnh,
                        Gia = c.Gia,
                        SoLuong = c.SoLuong,
                        SoLuongTon = c.Sach.SoLuong
                    }).ToList();
                    model.TongTien = model.GioHangItems.Sum(i => i.ThanhTien);
                }

                return View(model);
            }

            var gioHangCheck = await _context.GioHangs
                .FirstOrDefaultAsync(g => g.TaiKhoanId == userId.Value);

            if (gioHangCheck == null)
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "GioHang");
            }

            var chiTietGioHangCheck = await _context.ChiTietGioHangs
                .Where(c => c.GioHangId == gioHangCheck.Id)
                .Include(c => c.Sach)
                .ToListAsync();

            if (!chiTietGioHangCheck.Any())
            {
                TempData["ErrorMessage"] = "Giỏ hàng của bạn đang trống";
                return RedirectToAction("Index", "GioHang");
            }

            // Kiểm tra số lượng tồn kho
            foreach (var item in chiTietGioHangCheck)
            {
                if (item.SoLuong > item.Sach.SoLuong)
                {
                    TempData["ErrorMessage"] = $"Sách '{item.Sach.TenSach}' chỉ còn {item.Sach.SoLuong} cuốn";
                    return RedirectToAction("Index", "GioHang");
                }
            }

            // Tạo đơn hàng
            var donHang = new DonHang
            {
                TaiKhoanId = userId.Value,
                NgayDat = DateTime.Now,
                TongTien = chiTietGioHangCheck.Sum(c => c.Gia * c.SoLuong),
                TrangThai = "Chờ xử lý",
                HoTenNguoiNhan = model.HoTenNguoiNhan,
                SoDienThoai = model.SoDienThoai,
                DiaChiGiaoHang = model.DiaChiGiaoHang,
                GhiChu = model.GhiChu
            };

            _context.DonHangs.Add(donHang);
            await _context.SaveChangesAsync();

            // Tạo chi tiết đơn hàng và cập nhật số lượng tồn kho
            foreach (var item in chiTietGioHangCheck)
            {
                var chiTietDonHang = new ChiTietDonHang
                {
                    DonHangId = donHang.Id,
                    SachId = item.SachId,
                    SoLuong = item.SoLuong,
                    Gia = item.Gia
                };
                _context.ChiTietDonHangs.Add(chiTietDonHang);

                // Giảm số lượng tồn kho
                item.Sach.SoLuong -= item.SoLuong;
            }

            // Xóa giỏ hàng
            _context.ChiTietGioHangs.RemoveRange(chiTietGioHangCheck);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đặt hàng thành công! Cảm ơn bạn đã mua sắm.";
            return RedirectToAction("Details", new { id = donHang.Id });
        }

        public async Task<IActionResult> Index()
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để xem đơn hàng";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            var donHangs = await _context.DonHangs
                .Where(d => d.TaiKhoanId == userId.Value)
                .OrderByDescending(d => d.NgayDat)
                .ToListAsync();

            return View(donHangs);
        }

        public async Task<IActionResult> Details(int? id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (id == null)
            {
                return NotFound();
            }

            var donHang = await _context.DonHangs
                .Include(d => d.TaiKhoan)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.Sach)
                .FirstOrDefaultAsync(d => d.Id == id && d.TaiKhoanId == userId.Value);

            if (donHang == null)
            {
                return NotFound();
            }

            return View(donHang);
        }

        [HttpPost]
        public async Task<IActionResult> Cancel(int id)
        {
            var userId = GetUserId();
            if (!userId.HasValue)
            {
                return Json(new { success = false, message = "Vui lòng đăng nhập" });
            }

            var donHang = await _context.DonHangs
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.Sach)
                .FirstOrDefaultAsync(d => d.Id == id && d.TaiKhoanId == userId.Value);

            if (donHang == null)
            {
                return Json(new { success = false, message = "Không tìm thấy đơn hàng" });
            }

            if (donHang.TrangThai != "Chờ xử lý")
            {
                return Json(new { success = false, message = "Chỉ có thể hủy đơn hàng đang chờ xử lý" });
            }

            // Hoàn lại số lượng tồn kho
            foreach (var chiTiet in donHang.ChiTietDonHangs)
            {
                chiTiet.Sach.SoLuong += chiTiet.SoLuong;
            }

            donHang.TrangThai = "Đã hủy";
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Đã hủy đơn hàng thành công" });
        }
    }
}

