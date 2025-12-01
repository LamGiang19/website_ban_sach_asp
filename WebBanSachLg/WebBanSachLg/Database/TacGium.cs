using System;
using System.Collections.Generic;

namespace WebBanSachLg.Database;

public partial class TacGium
{
    public int Id { get; set; }

    public string TenTacGia { get; set; } = null!;

    public string? GioiThieu { get; set; }

    public DateTime? NgayTao { get; set; }

    public bool? TrangThai { get; set; }

    public virtual ICollection<Sach> Saches { get; } = new List<Sach>();
}
