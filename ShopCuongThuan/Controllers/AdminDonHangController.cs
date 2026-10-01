using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;

namespace ShopCuongThuan.Controllers
{
    public class AdminDonHangController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminDonHangController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var query = _context.DonHang
                .Include(dh => dh.KhachHang)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(dh =>
                    dh.MaTraCuu.Contains(search) ||
                    dh.HoTenNhan.Contains(search) ||
                    dh.SoDienThoai.Contains(search) ||
                    dh.TrangThai.Contains(search) ||
                    dh.PhuongThucThanhToan!.Contains(search));
            }

            ViewBag.Search = search;

            var donHangs = query
                .OrderByDescending(dh => dh.NgayDat)
                .ToList();

            return View(donHangs);
        }

        public IActionResult Details(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var donHang = _context.DonHang
                .Include(dh => dh.KhachHang)
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefault(dh => dh.MaDH == id);

            if (donHang == null)
            {
                return RedirectToAction("Index");
            }

            return View(donHang);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int maDH, string trangThai)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var donHang = _context.DonHang.Find(maDH);

            if (donHang != null)
            {
                donHang.TrangThai = trangThai;
                _context.SaveChanges();
            }

            return RedirectToAction("Details", new { id = maDH });
        }
    }
}