using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class DanhMuc
{
    public int Id { get; set; }

    public string TenDanhMuc { get; set; } = null!;

    public string? MoTa { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<Sach> Saches { get; } = new List<Sach>();
}
