using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;

namespace ShopCuongThuan.Controllers
{
    public class AdminThongKeController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminThongKeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(DateTime? ngay, string? thangNam)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var query = _context.DonHang
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .Where(dh => dh.TrangThai == "Hoàn thành")
                .AsQueryable();

            string kieuThongKe;
            string tieuDeThongKe;
            DateTime startDate;
            DateTime endDate;

            // Nếu chọn tháng thì lọc theo tháng
            if (!string.IsNullOrWhiteSpace(thangNam))
            {
                var parts = thangNam.Split('-');
                int nam = int.Parse(parts[0]);
                int thang = int.Parse(parts[1]);

                startDate = new DateTime(nam, thang, 1);
                endDate = startDate.AddMonths(1);

                kieuThongKe = "Thang";
                tieuDeThongKe = $"Doanh thu tháng {thang}/{nam}";
            }
            else
            {
                // Nếu không chọn gì thì mặc định là hôm nay
                var ngayCanXem = ngay ?? DateTime.Today;

                startDate = ngayCanXem.Date;
                endDate = startDate.AddDays(1);

                kieuThongKe = "Ngay";

                if (startDate == DateTime.Today)
                {
                    tieuDeThongKe = "Doanh thu hôm nay";
                }
                else
                {
                    tieuDeThongKe = $"Doanh thu ngày {startDate:dd/MM/yyyy}";
                }
            }

            var donHangs = query
                .Where(dh => dh.NgayDat >= startDate && dh.NgayDat < endDate)
                .OrderByDescending(dh => dh.NgayDat)
                .ToList();

            ViewBag.TongDoanhThu = donHangs.Sum(dh => dh.TongTien);
            ViewBag.SoDonHang = donHangs.Count;
            ViewBag.SoSanPhamBan = donHangs
                .SelectMany(dh => dh.ChiTietDonHangs)
                .Sum(ct => ct.SoLuong);

            ViewBag.KieuThongKe = kieuThongKe;
            ViewBag.TieuDeThongKe = tieuDeThongKe;
            ViewBag.Ngay = startDate.ToString("yyyy-MM-dd");
            ViewBag.ThangNam = !string.IsNullOrWhiteSpace(thangNam)
                ? thangNam
                : DateTime.Today.ToString("yyyy-MM");

            return View(donHangs);
        }
    }
}