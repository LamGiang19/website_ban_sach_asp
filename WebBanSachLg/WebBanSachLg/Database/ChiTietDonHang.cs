using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class ChiTietDonHang
{
    public int Id { get; set; }

    public int DonHangId { get; set; }

    public int SachId { get; set; }

    public int SoLuong { get; set; }

    public decimal Gia { get; set; }

    public virtual DonHang DonHang { get; set; } = null!;

    public virtual Sach Sach { get; set; } = null!;
}
