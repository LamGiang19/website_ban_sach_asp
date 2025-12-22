using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using System.Security.Cryptography;
using System.Text;
using WebBanSachLg.Database;
using WebBanSachLg.Helpers;
using WebBanSachLg.Models;
using WebBanSachLg.Services;

namespace WebBanSachLg.Controllers
{
    public class TaiKhoanController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<TaiKhoanController> _logger;
        private readonly IMemoryCache _cache;
        private readonly IEmailService _emailService;

        public TaiKhoanController(WebBanSachDbContext context, ILogger<TaiKhoanController> logger, IMemoryCache cache, IEmailService emailService)
        {
            _context = context;
            _logger = logger;
            _cache = cache;
            _emailService = emailService;
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

        public IActionResult QuenMatKhau()
        {
            if (IsLoggedIn())
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuenMatKhau(QuenMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var taiKhoan = await _context.TaiKhoans
                .FirstOrDefaultAsync(t => t.Email == model.Email);

            if (taiKhoan == null)
            {
                // Không tiết lộ email có tồn tại hay không (bảo mật)
                TempData["SuccessMessage"] = "Nếu email tồn tại trong hệ thống, bạn sẽ nhận được link đặt lại mật khẩu.";
                return RedirectToAction("QuenMatKhau");
            }

            if (taiKhoan.TrangThai != true)
            {
                ModelState.AddModelError("", "Tài khoản đã bị khóa");
                return View(model);
            }

            // Tạo token reset
            var token = GenerateResetToken();
            var cacheKey = $"ResetPassword_{taiKhoan.Email}_{token}";
            
            // Lưu token vào cache với thời gian hết hạn 1 giờ
            _cache.Set(cacheKey, taiKhoan.Id, TimeSpan.FromHours(1));

            // Tạo link reset
            var resetLink = Url.Action("ResetMatKhau", "TaiKhoan", new { email = taiKhoan.Email, token = token }, Request.Scheme);
            
            if (string.IsNullOrEmpty(resetLink))
            {
                ModelState.AddModelError("", "Không thể tạo link đặt lại mật khẩu. Vui lòng thử lại.");
                return View(model);
            }

            // Gửi email đặt lại mật khẩu
            var emailSent = await _emailService.SendPasswordResetEmailAsync(
                taiKhoan.Email, 
                resetLink, 
                taiKhoan.HoTen ?? taiKhoan.TenDangNhap
            );

            if (emailSent)
            {
                TempData["SuccessMessage"] = "Email đặt lại mật khẩu đã được gửi đến địa chỉ email của bạn. Vui lòng kiểm tra hộp thư.";
            }
            else
            {
                // Nếu không gửi được email, vẫn hiển thị link (fallback cho development)
                TempData["ResetLink"] = resetLink;
                TempData["Email"] = taiKhoan.Email;
                TempData["WarningMessage"] = "Không thể gửi email. Vui lòng sử dụng link bên dưới để đặt lại mật khẩu.";
            }

            return RedirectToAction("QuenMatKhauSuccess");
        }

        public IActionResult QuenMatKhauSuccess()
        {
            // Lấy dữ liệu từ TempData (có thể null nếu email đã được gửi thành công)
            ViewBag.ResetLink = TempData["ResetLink"] as string;
            ViewBag.Email = TempData["Email"] as string;
            
            return View();
        }

        public IActionResult ResetMatKhau(string? email, string? token)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(token))
            {
                TempData["ErrorMessage"] = "Link đặt lại mật khẩu không hợp lệ";
                return RedirectToAction("QuenMatKhau");
            }

            var cacheKey = $"ResetPassword_{email}_{token}";
            if (!_cache.TryGetValue(cacheKey, out int userId))
            {
                TempData["ErrorMessage"] = "Link đặt lại mật khẩu đã hết hạn hoặc không hợp lệ";
                return RedirectToAction("QuenMatKhau");
            }

            var model = new ResetMatKhauViewModel
            {
                Email = email,
                Token = token
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetMatKhau(ResetMatKhauViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var cacheKey = $"ResetPassword_{model.Email}_{model.Token}";
            if (!_cache.TryGetValue(cacheKey, out int userId))
            {
                TempData["ErrorMessage"] = "Link đặt lại mật khẩu đã hết hạn hoặc không hợp lệ";
                return RedirectToAction("QuenMatKhau");
            }

            var taiKhoan = await _context.TaiKhoans.FindAsync(userId);
            if (taiKhoan == null || taiKhoan.Email != model.Email)
            {
                TempData["ErrorMessage"] = "Tài khoản không tồn tại";
                return RedirectToAction("QuenMatKhau");
            }

            // Cập nhật mật khẩu mới
            taiKhoan.MatKhau = PasswordHelper.HashPassword(model.MatKhauMoi);
            await _context.SaveChangesAsync();

            // Xóa token khỏi cache
            _cache.Remove(cacheKey);

            TempData["SuccessMessage"] = "Đặt lại mật khẩu thành công! Vui lòng đăng nhập với mật khẩu mới.";
            return RedirectToAction("DangNhap");
        }

        private string GenerateResetToken()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                var bytes = new byte[32];
                rng.GetBytes(bytes);
                return Convert.ToBase64String(bytes)
                    .Replace("+", "-")
                    .Replace("/", "_")
                    .Replace("=", "");
            }
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

