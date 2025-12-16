using WebBanSachLg.Database;

namespace WebBanSachLg.Models
{
    public class TimKiemViewModel
    {
        public string? Keyword { get; set; }
        public int? DanhMucId { get; set; }
        public int? TacGiaId { get; set; }
        public decimal? GiaMin { get; set; }
        public decimal? GiaMax { get; set; }
        public string? SapXep { get; set; } // "gia-tang", "gia-giam", "ten", "moi-nhat"
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;
        public int TotalItems { get; set; }
        public int TotalPages => (int)Math.Ceiling(TotalItems / (double)PageSize);
        public List<Sach> Saches { get; set; } = new();
        public List<DanhMuc> DanhMucs { get; set; } = new();
        public List<TacGium> TacGias { get; set; } = new();
    }
}

