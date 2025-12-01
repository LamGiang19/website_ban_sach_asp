using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class Sach
{
    public int Id { get; set; }

    public string TenSach { get; set; } = null!;

    public string? MoTa { get; set; }

    public decimal Gia { get; set; }

    public int SoLuong { get; set; }

    public string? HinhAnh { get; set; }

    public int DanhMucId { get; set; }

    public int TacGiaId { get; set; }

    public int NhaXuatBanId { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<ChiTietDonHang> ChiTietDonHangs { get; } = new List<ChiTietDonHang>();

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; } = new List<ChiTietGioHang>();

    public virtual DanhMuc DanhMuc { get; set; } = null!;

    public virtual NhaXuatBan NhaXuatBan { get; set; } = null!;

    public virtual TacGium TacGia { get; set; } = null!;
}
