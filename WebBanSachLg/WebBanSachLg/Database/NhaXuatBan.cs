using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class NhaXuatBan
{
    public int Id { get; set; }

    public string TenNhaXuatBan { get; set; } = null!;

    public string? DiaChi { get; set; }

    public string? SoDienThoai { get; set; }

    public string? Email { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<Sach> Saches { get; } = new List<Sach>();
}
