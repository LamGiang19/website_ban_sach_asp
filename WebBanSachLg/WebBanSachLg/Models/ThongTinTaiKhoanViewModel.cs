using System.ComponentModel.DataAnnotations;

namespace WebBanSachLg.Models
{
    public class ThongTinTaiKhoanViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [Display(Name = "Họ tên")]
        [StringLength(255)]
        public string HoTen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email là bắt buộc")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string? SoDienThoai { get; set; }

        [Display(Name = "Địa chỉ")]
        [StringLength(500)]
        public string? DiaChi { get; set; }

        [Display(Name = "Tên đăng nhập")]
        public string TenDangNhap { get; set; } = string.Empty;
    }

    public class DoiMatKhauViewModel
    {
        [Required(ErrorMessage = "Mật khẩu hiện tại là bắt buộc")]
        [Display(Name = "Mật khẩu hiện tại")]
        [DataType(DataType.Password)]
        public string MatKhauHienTai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Mật khẩu mới là bắt buộc")]
        [Display(Name = "Mật khẩu mới")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 đến 100 ký tự")]
        [DataType(DataType.Password)]
        public string MatKhauMoi { get; set; } = string.Empty;

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [Display(Name = "Xác nhận mật khẩu mới")]
        [DataType(DataType.Password)]
        [Compare("MatKhauMoi", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string XacNhanMatKhauMoi { get; set; } = string.Empty;
    }
}

