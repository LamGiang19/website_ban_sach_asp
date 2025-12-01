using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class TaiKhoan
{
    public int Id { get; set; }

    public string TenDangNhap { get; set; } = null!;

    public string MatKhau { get; set; } = null!;

    public string HoTen { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? SoDienThoai { get; set; }

    public string? DiaChi { get; set; }

    public string? VaiTro { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<DonHang> DonHangs { get; } = new List<DonHang>();

    public virtual ICollection<GioHang> GioHangs { get; } = new List<GioHang>();
}
