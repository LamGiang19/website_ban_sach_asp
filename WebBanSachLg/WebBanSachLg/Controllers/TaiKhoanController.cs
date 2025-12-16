using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;
using WebBanSachLg.Helpers;
using WebBanSachLg.Models;

namespace WebBanSachLg.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<TaiKhoanController> _logger;

        public TaiKhoanController(WebBanSachDbContext context, ILogger<TaiKhoanController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult DangKy()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangKy(DangKyViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var existingUser = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

            if (existingUser != null)
            {
                ModelState.AddModelError("TenDangNhap", "Tên đăng nhập đã tồn tại");
                return View(model);
            }

            var existingEmail = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Email == model.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError("Email", "Email đã được sử dụng");
                return View(model);
            }

            var taiKhoan = new TaiKhoan
            {
                TenDangNhap = model.TenDangNhap,
                MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                HoTen = model.HoTen,
                Email = model.Email,
                SoDienThoai = model.SoDienThoai,
                DiaChi = model.DiaChi,
                VaiTro = "User",
                NgayTao = DateTime.Now,
                TrangThai = true
            };

            _context.TaiKhoans.Add(taiKhoan);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký thành công! Vui lòng đăng nhập.";
            return RedirectToAction("DangNhap");
        }

        public IActionResult DangNhap()
        {
            if (IsLoggedIn())
            {
                return RedirectToHome();
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DangNhap(DangNhapViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.TenDangNhap == model.TenDangNhap);

            if (taiKhoan == null)
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            if (!PasswordHelper.VerifyPassword(model.MatKhau, taiKhoan.MatKhau))
            {
                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
                return View(model);
            }

            if (taiKhoan.TrangThai != true)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa");
                return View(model);
            }

            HttpContext.Session.SetString("UserId", taiKhoan.Id.ToString());
            HttpContext.Session.SetString("TenDangNhap", taiKhoan.TenDangNhap);
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("VaiTro", taiKhoan.VaiTro ?? "User");
            HttpContext.Session.SetString("Email", taiKhoan.Email);

            if (model.GhiNho)
            {
            }

            TempData["SuccessMessage"] = $"Chào mừng {taiKhoan.HoTen}!";
            
            return RedirectToHome();
        }

        public IActionResult DangXuat()
        {
            HttpContext.Session.Clear();
            TempData["SuccessMessage"] = "Đã đăng xuất thành công";
            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> ThongTin()
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản";
                return RedirectToAction("DangNhap");
            }

            var model = new ThongTinTaiKhoanViewModel
            {
                Id = taiKhoan.Id,
                TenDangNhap = taiKhoan.TenDangNhap,
                HoTen = taiKhoan.HoTen,
                Email = taiKhoan.Email,
                SoDienThoai = taiKhoan.SoDienThoai,
                DiaChi = taiKhoan.DiaChi
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ThongTin(ThongTinTaiKhoanViewModel model)
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản";
                return RedirectToAction("DangNhap");
            }

            // Kiểm tra email trùng (nếu thay đổi)
            if (taiKhoan.Email != model.Email)
            {
                var existingEmail = await _context.TaiKhoans
                    .FirstOrDefaultAsync(t => t.Email == model.Email && t.Id != userId);
                if (existingEmail != null)
                {
                    ModelState.AddModelError("Email", "Email đã được sử dụng");
                    return View(model);
                }
            }

            taiKhoan.HoTen = model.HoTen;
            taiKhoan.Email = model.Email;
            taiKhoan.SoDienThoai = model.SoDienThoai;
            taiKhoan.DiaChi = model.DiaChi;

            await _context.SaveChangesAsync();

            // Cập nhật session
            HttpContext.Session.SetString("HoTen", taiKhoan.HoTen);
            HttpContext.Session.SetString("Email", taiKhoan.Email);

            TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            return RedirectToAction("ThongTin");
        }

        public IActionResult DoiMatKhau()
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DoiMatKhau(DoiMatKhauViewModel model)
        {
            if (!IsLoggedIn())
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr) || !int.TryParse(userIdStr, out var userId))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap");
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            if (taiKhoan == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy tài khoản";
                return RedirectToAction("DangNhap");
            }

            // Kiểm tra mật khẩu hiện tại
            if (!PasswordHelper.VerifyPassword(model.MatKhauHienTai, taiKhoan.MatKhau))
            {
                ModelState.AddModelError("MatKhauHienTai", "Mật khẩu hiện tại không đúng");
                return View(model);
            }

            // Cập nhật mật khẩu mới
            taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đổi mật khẩu thành công!";
            return RedirectToAction("DoiMatKhau");
        }

        private bool IsLoggedIn()
        {
            return !string.IsNullOrEmpty(HttpContext.Session.GetString("UserId"));
        }

        private IActionResult RedirectToHome()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            
            if (vaiTro == "Admin")
            {
                return RedirectToAction("Index", "Admin");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
    }
}

