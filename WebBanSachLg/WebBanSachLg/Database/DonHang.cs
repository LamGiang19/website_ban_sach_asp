using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class DonHang
{
    public int Id { get; set; }

    public int TaiKhoanId { get; set; }

    public DateTime? NgayDat { get; set; }

    public decimal TongTien { get; set; }

    public string? TrangThai { get; set; }

    public string DiaChiGiaoHang { get; set; } = null!;

    public string SoDienThoai { get; set; } = null!;

    public string HoTenNguoiNhan { get; set; } = null!;

    public string? GhiChu { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; } = new List<ChiTietDonHang>();

    public virtual TaiKhoan TaiKhoan { get; set; } = null!;
}
