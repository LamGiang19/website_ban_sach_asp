using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class GioHang
{
    public int Id { get; set; }

    public int TaiKhoanId { get; set; }

    public DateTime? NgayTao { get; set; }

    public virtual ICollection<ChiTietGioHang> ChiTietGioHangs { get; } = new List<ChiTietGioHang>();

    public virtual TaiKhoan TaiKhoan { get; set; } = null!;
}
