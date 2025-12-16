using WebBanSachLg.Database;

namespace WebBanSachLg.Models
{
    public class GioHangViewModel
    {
        public int Id { get; set; }
        public int SachId { get; set; }
        public string TenSach { get; set; } = string.Empty;
        public string? HinhAnh { get; set; }
        public decimal Gia { get; set; }
        public int SoLuong { get; set; }
        public decimal ThanhTien => Gia * SoLuong;
        public int SoLuongTon { get; set; }
    }

    public class GioHangIndexViewModel
    {
        public List<GioHangViewModel> Items { get; set; } = new();
        public decimal TongTien => Items.Sum(i => i.ThanhTien);
    }
}

