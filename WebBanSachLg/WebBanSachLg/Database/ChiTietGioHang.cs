using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class ChiTietGioHang
{
    public int Id { get; set; }

    public int GioHangId { get; set; }

    public int SachId { get; set; }

    public int SoLuong { get; set; }

    public decimal Gia { get; set; }

    public DateTime? NgayThem { get; set; }

    public virtual GioHang GioHang { get; set; } = null!;

    public virtual Sach Sach { get; set; } = null!;
}
