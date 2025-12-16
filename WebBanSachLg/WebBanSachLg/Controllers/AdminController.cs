using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;
using WebBanSachLg.Helpers;

namespace WebBanSachLg.Controllers
{
    public class AdminController : Controller
    {
        private readonly WebBanSachDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(WebBanSachDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        private bool IsAdmin()
        {
            var vaiTro = HttpContext.Session.GetString("VaiTro");
            return vaiTro == "Admin";
        }

        private IActionResult CheckAdmin()
        {
            if (string.IsNullOrEmpty(HttpContext.Session.GetString("UserId")))
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập";
                return RedirectToAction("DangNhap", "TaiKhoan");
            }

            if (!IsAdmin())
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập trang này";
                return RedirectToAction("Index", "Home");
            }

            return null!;
        }

        public IActionResult Index()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var tongSach = _context.Saches.Count();
            var tongDonHang = _context.DonHangs.Count();
            var tongTaiKhoan = _context.TaiKhoans.Count();
            var tongDoanhThu = _context.DonHangs
                .Where(d => d.TrangThai == "Đã giao")
                .Sum(d => (decimal?)d.TongTien) ?? 0;

            ViewBag.TongSach = tongSach;
            ViewBag.TongDonHang = tongDonHang;
            ViewBag.TongTaiKhoan = tongTaiKhoan;
            ViewBag.TongDoanhThu = tongDoanhThu;

            var donHangMoi = _context.DonHangs
                .Include(d => d.TaiKhoan)
                .OrderByDescending(d => d.NgayDat)
                .Take(10)
                .ToList();

            ViewBag.DonHangMoi = donHangMoi;

            return View();
        }

        public IActionResult DanhMuc()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var danhMucs = _context.DanhMucs.OrderBy(d => d.TenDanhMuc).ToList();
            return View(danhMucs);
        }

        [HttpGet]
        public IActionResult DanhMucCreate()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhMucCreate(DanhMuc danhMuc)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (ModelState.IsValid)
            {
                danhMuc.NgayTao = DateTime.Now;
                var trangThaiValue = Request.Form["TrangThai"].ToString();
                danhMuc.TrangThai = trangThaiValue == "true";
                _context.DanhMucs.Add(danhMuc);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm danh mục thành công!";
                return RedirectToAction("DanhMuc");
            }
            return View(danhMuc);
        }

        [HttpGet]
        public async Task<IActionResult> DanhMucEdit(int? id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id == null) return NotFound();

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc == null) return NotFound();

            return View(danhMuc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DanhMucEdit(int id, DanhMuc danhMuc)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id != danhMuc.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingDanhMuc = await _context.DanhMucs.FindAsync(id);
                    if (existingDanhMuc == null) return NotFound();

                    existingDanhMuc.TenDanhMuc = danhMuc.TenDanhMuc;
                    existingDanhMuc.MoTa = danhMuc.MoTa;
                    
                    var trangThaiValue = Request.Form["TrangThai"].ToString();
                    existingDanhMuc.TrangThai = trangThaiValue == "true";
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật danh mục thành công!";
                    return RedirectToAction("DanhMuc");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.DanhMucs.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
            }
            
            ViewBag.DanhMucs = new SelectList(_context.DanhMucs.Where(d => d.TrangThai == true), "Id", "TenDanhMuc", danhMuc.Id);
            return View(danhMuc);
        }

        [HttpPost]
        public async Task<IActionResult> DanhMucDelete(int id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var danhMuc = await _context.DanhMucs.FindAsync(id);
            if (danhMuc != null)
            {
                var hasBooks = await _context.Saches.AnyAsync(s => s.DanhMucId == id);
                if (hasBooks)
                {
                    TempData["ErrorMessage"] = "Không thể xóa danh mục này vì đang có sách sử dụng!";
                }
                else
                {
                    _context.DanhMucs.Remove(danhMuc);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa danh mục thành công!";
                }
            }
            return RedirectToAction("DanhMuc");
        }

        public IActionResult TacGia()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var tacGias = _context.TacGia.OrderBy(t => t.TenTacGia).ToList();
            return View(tacGias);
        }

        [HttpGet]
        public IActionResult TacGiaCreate()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TacGiaCreate(TacGium tacGia)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (ModelState.IsValid)
            {
                tacGia.NgayTao = DateTime.Now;
                var trangThaiValue = Request.Form["TrangThai"].ToString();
                tacGia.TrangThai = trangThaiValue == "true";
                _context.TacGia.Add(tacGia);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm tác giả thành công!";
                return RedirectToAction("TacGia");
            }
            return View(tacGia);
        }

        [HttpGet]
        public async Task<IActionResult> TacGiaEdit(int? id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id == null) return NotFound();

            var tacGia = await _context.TacGia.FindAsync(id);
            if (tacGia == null) return NotFound();

            return View(tacGia);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TacGiaEdit(int id, TacGium tacGia)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id != tacGia.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingTacGia = await _context.TacGia.FindAsync(id);
                    if (existingTacGia == null) return NotFound();

                    existingTacGia.TenTacGia = tacGia.TenTacGia;
                    existingTacGia.GioiThieu = tacGia.GioiThieu;
                    
                    var trangThaiValue = Request.Form["TrangThai"].ToString();
                    existingTacGia.TrangThai = trangThaiValue == "true";
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật tác giả thành công!";
                    return RedirectToAction("TacGia");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.TacGia.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
            }
            return View(tacGia);
        }

        [HttpPost]
        public async Task<IActionResult> TacGiaDelete(int id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var tacGia = await _context.TacGia.FindAsync(id);
            if (tacGia != null)
            {
                var hasBooks = await _context.Saches.AnyAsync(s => s.TacGiaId == id);
                if (hasBooks)
                {
                    TempData["ErrorMessage"] = "Không thể xóa tác giả này vì đang có sách sử dụng!";
                }
                else
                {
                    _context.TacGia.Remove(tacGia);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa tác giả thành công!";
                }
            }
            return RedirectToAction("TacGia");
        }

        public IActionResult NhaXuatBan()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var nhaXuatBans = _context.NhaXuatBans.OrderBy(n => n.TenNhaXuatBan).ToList();
            return View(nhaXuatBans);
        }

        [HttpGet]
        public IActionResult NhaXuatBanCreate()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NhaXuatBanCreate(NhaXuatBan nhaXuatBan)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (ModelState.IsValid)
            {
                nhaXuatBan.NgayTao = DateTime.Now;
                var trangThaiValue = Request.Form["TrangThai"].ToString();
                nhaXuatBan.TrangThai = trangThaiValue == "true";
                _context.NhaXuatBans.Add(nhaXuatBan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Thêm nhà xuất bản thành công!";
                return RedirectToAction("NhaXuatBan");
            }
            return View(nhaXuatBan);
        }

        [HttpGet]
        public async Task<IActionResult> NhaXuatBanEdit(int? id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id == null) return NotFound();

            var nhaXuatBan = await _context.NhaXuatBans.FindAsync(id);
            if (nhaXuatBan == null) return NotFound();

            return View(nhaXuatBan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> NhaXuatBanEdit(int id, NhaXuatBan nhaXuatBan)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id != nhaXuatBan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingNhaXuatBan = await _context.NhaXuatBans.FindAsync(id);
                    if (existingNhaXuatBan == null) return NotFound();

                    existingNhaXuatBan.TenNhaXuatBan = nhaXuatBan.TenNhaXuatBan;
                    existingNhaXuatBan.DiaChi = nhaXuatBan.DiaChi;
                    existingNhaXuatBan.SoDienThoai = nhaXuatBan.SoDienThoai;
                    existingNhaXuatBan.Email = nhaXuatBan.Email;
                    
                    var trangThaiValue = Request.Form["TrangThai"].ToString();
                    existingNhaXuatBan.TrangThai = trangThaiValue == "true";
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật nhà xuất bản thành công!";
                    return RedirectToAction("NhaXuatBan");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.NhaXuatBans.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
            }
            return View(nhaXuatBan);
        }

        [HttpPost]
        public async Task<IActionResult> NhaXuatBanDelete(int id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var nhaXuatBan = await _context.NhaXuatBans.FindAsync(id);
            if (nhaXuatBan != null)
            {
                var hasBooks = await _context.Saches.AnyAsync(s => s.NhaXuatBanId == id);
                if (hasBooks)
                {
                    TempData["ErrorMessage"] = "Không thể xóa nhà xuất bản này vì đang có sách sử dụng!";
                }
                else
                {
                    _context.NhaXuatBans.Remove(nhaXuatBan);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa nhà xuất bản thành công!";
                }
            }
            return RedirectToAction("NhaXuatBan");
        }

        public IActionResult Sach()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var saches = _context.Saches
                .Include(s => s.DanhMuc)
                .Include(s => s.TacGia)
                .Include(s => s.NhaXuatBan)
                .OrderByDescending(s => s.NgayTao)
                .ToList();
            return View(saches);
        }

        [HttpGet]
        public IActionResult SachCreate()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            ViewBag.DanhMucs = new SelectList(_context.DanhMucs.Where(d => d.TrangThai == true), "Id", "TenDanhMuc");
            ViewBag.TacGias = new SelectList(_context.TacGia.Where(t => t.TrangThai == true), "Id", "TenTacGia");
            ViewBag.NhaXuatBans = new SelectList(_context.NhaXuatBans.Where(n => n.TrangThai == true), "Id", "TenNhaXuatBan");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SachCreate(Sach sach, IFormFile? hinhAnhFile)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (ModelState.IsValid)
            {
                try
                {
                    if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                    {
                        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        var imageFileName = await FileUploadHelper.UploadImageAsync(hinhAnhFile, webRootPath, "images/sach");
                        sach.HinhAnh = imageFileName;
                    }
                    else if (!string.IsNullOrEmpty(sach.HinhAnh))
                    {
                        var imagePath = sach.HinhAnh.Trim();
                        if (imagePath.Contains('/') || imagePath.Contains('\\'))
                        {
                            sach.HinhAnh = Path.GetFileName(imagePath);
                        }
                    }
                    else
                    {
                        sach.HinhAnh = null;
                    }

                    sach.NgayTao = DateTime.Now;
                    var trangThaiValue = Request.Form["TrangThai"].ToString();
                    sach.TrangThai = trangThaiValue == "true";
                    
                    _context.Saches.Add(sach);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Thêm sách thành công!";
                    return RedirectToAction("Sach");
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.DanhMucs = new SelectList(_context.DanhMucs.Where(d => d.TrangThai == true), "Id", "TenDanhMuc", sach.DanhMucId);
            ViewBag.TacGias = new SelectList(_context.TacGia.Where(t => t.TrangThai == true), "Id", "TenTacGia", sach.TacGiaId);
            ViewBag.NhaXuatBans = new SelectList(_context.NhaXuatBans.Where(n => n.TrangThai == true), "Id", "TenNhaXuatBan", sach.NhaXuatBanId);
            return View(sach);
        }

        [HttpGet]
        public async Task<IActionResult> SachEdit(int? id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id == null) return NotFound();

            var sach = await _context.Saches.FindAsync(id);
            if (sach == null) return NotFound();

            ViewBag.DanhMucs = new SelectList(_context.DanhMucs.Where(d => d.TrangThai == true), "Id", "TenDanhMuc", sach.DanhMucId);
            ViewBag.TacGias = new SelectList(_context.TacGia.Where(t => t.TrangThai == true), "Id", "TenTacGia", sach.TacGiaId);
            ViewBag.NhaXuatBans = new SelectList(_context.NhaXuatBans.Where(n => n.TrangThai == true), "Id", "TenNhaXuatBan", sach.NhaXuatBanId);
            return View(sach);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SachEdit(int id, Sach sach, IFormFile? hinhAnhFile)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id != sach.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var existingSach = await _context.Saches.FindAsync(id);
                    if (existingSach == null) return NotFound();

                    var oldImageFileName = existingSach.HinhAnh;

                    if (hinhAnhFile != null && hinhAnhFile.Length > 0)
                    {
                        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        
                        if (!string.IsNullOrEmpty(oldImageFileName))
                        {
                            FileUploadHelper.DeleteImage(oldImageFileName, webRootPath, "images/sach");
                        }
                        
                        var imageFileName = await FileUploadHelper.UploadImageAsync(hinhAnhFile, webRootPath, "images/sach");
                        existingSach.HinhAnh = imageFileName;
                    }
                    else if (!string.IsNullOrEmpty(sach.HinhAnh))
                    {
                        var imagePath = sach.HinhAnh.Trim();
                        if (imagePath.Contains('/') || imagePath.Contains('\\'))
                        {
                            existingSach.HinhAnh = Path.GetFileName(imagePath);
                        }
                        else
                        {
                            existingSach.HinhAnh = imagePath;
                        }
                    }

                    existingSach.TenSach = sach.TenSach;
                    existingSach.MoTa = sach.MoTa;
                    existingSach.Gia = sach.Gia;
                    existingSach.SoLuong = sach.SoLuong;
                    existingSach.DanhMucId = sach.DanhMucId;
                    existingSach.TacGiaId = sach.TacGiaId;
                    existingSach.NhaXuatBanId = sach.NhaXuatBanId;

                    var trangThaiValue = Request.Form["TrangThai"].ToString();
                    existingSach.TrangThai = trangThaiValue == "true";
                    
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Cập nhật sách thành công!";
                    return RedirectToAction("Sach");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Saches.Any(e => e.Id == id))
                        return NotFound();
                    throw;
                }
                catch (ArgumentException ex)
                {
                    ModelState.AddModelError("", ex.Message);
                }
            }

            ViewBag.DanhMucs = new SelectList(_context.DanhMucs.Where(d => d.TrangThai == true), "Id", "TenDanhMuc", sach.DanhMucId);
            ViewBag.TacGias = new SelectList(_context.TacGia.Where(t => t.TrangThai == true), "Id", "TenTacGia", sach.TacGiaId);
            ViewBag.NhaXuatBans = new SelectList(_context.NhaXuatBans.Where(n => n.TrangThai == true), "Id", "TenNhaXuatBan", sach.NhaXuatBanId);
            return View(sach);
        }

        [HttpPost]
        public async Task<IActionResult> SachDelete(int id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var sach = await _context.Saches.FindAsync(id);
            if (sach != null)
            {
                var hasOrders = await _context.ChiTietDonHangs.AnyAsync(c => c.SachId == id);
                if (hasOrders)
                {
                    TempData["ErrorMessage"] = "Không thể xóa sách này vì đang có trong đơn hàng!";
                }
                else
                {
                    if (!string.IsNullOrEmpty(sach.HinhAnh))
                    {
                        var webRootPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                        FileUploadHelper.DeleteImage(sach.HinhAnh, webRootPath, "images/sach");
                    }
                    
                    _context.Saches.Remove(sach);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa sách thành công!";
                }
            }
            return RedirectToAction("Sach");
        }

        public IActionResult DonHang()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var donHangs = _context.DonHangs
                .Include(d => d.TaiKhoan)
                .OrderByDescending(d => d.NgayDat)
                .ToList();
            return View(donHangs);
        }

        [HttpGet]
        public async Task<IActionResult> DonHangDetails(int? id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            if (id == null) return NotFound();

            var donHang = await _context.DonHangs
                .Include(d => d.TaiKhoan)
                .Include(d => d.ChiTietDonHangs)
                    .ThenInclude(c => c.Sach)
                .FirstOrDefaultAsync(d => d.Id == id);

            if (donHang == null) return NotFound();

            return View(donHang);
        }

        [HttpPost]
        public async Task<IActionResult> DonHangUpdateStatus(int id, string trangThai)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var donHang = await _context.DonHangs.FindAsync(id);
            if (donHang != null)
            {
                donHang.TrangThai = trangThai;
                _context.Update(donHang);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật trạng thái đơn hàng thành công!";
            }
            return RedirectToAction("DonHang");
        }

        public IActionResult TaiKhoan()
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var taiKhoans = _context.TaiKhoans
                .OrderByDescending(t => t.NgayTao)
                .ToList();
            return View(taiKhoans);
        }

        [HttpPost]
        public async Task<IActionResult> TaiKhoanToggleStatus(int id)
        {
            var check = CheckAdmin();
            if (check != null) return check;

            var taiKhoan = await _context.TaiKhoans.FindAsync(id);
            if (taiKhoan != null)
            {
                taiKhoan.TrangThai = !taiKhoan.TrangThai;
                _context.Update(taiKhoan);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Cập nhật trạng thái tài khoản thành công!";
            }
            return RedirectToAction("TaiKhoan");
        }
    }
}
