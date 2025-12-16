using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanSachLg.Database;

namespace WebBanSachLg.ViewComponents
{
    public class DanhMucViewComponent : ViewComponent
    {
        private readonly WebBanSachDbContext _context;

        public DanhMucViewComponent(WebBanSachDbContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var danhMucs = await _context.DanhMucs
                .Where(d => d.TrangThai == true)
                .OrderBy(d => d.TenDanhMuc)
                .ToListAsync();

            return View(danhMucs);
        }
    }
}

