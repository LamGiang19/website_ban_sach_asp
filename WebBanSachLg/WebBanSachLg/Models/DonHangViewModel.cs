using System.ComponentModel.DataAnnotations;
using WebBanSachLg.Database;

namespace WebBanSachLg.Models
{
    public class DonHangCreateViewModel
    {
        [Required(ErrorMessage = "Họ tên người nhận là bắt buộc")]
        [Display(Name = "Họ tên người nhận")]
        [StringLength(255)]
        public string HoTenNguoiNhan { get; set; } = string.Empty;

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Display(Name = "Số điện thoại")]
        [StringLength(20)]
        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string SoDienThoai { get; set; } = string.Empty;

        [Required(ErrorMessage = "Địa chỉ giao hàng là bắt buộc")]
        [Display(Name = "Địa chỉ giao hàng")]
        [StringLength(500)]
        public string DiaChiGiaoHang { get; set; } = string.Empty;

        [Display(Name = "Ghi chú")]
        [StringLength(1000)]
        public string? GhiChu { get; set; }

        public List<GioHangViewModel> GioHangItems { get; set; } = new();
        public decimal TongTien { get; set; }
    }

    public class DonHangDetailsViewModel
    {
        public DonHang DonHang { get; set; } = null!;
        public List<ChiTietDonHang> ChiTietDonHangs { get; set; } = new();
    }
}

