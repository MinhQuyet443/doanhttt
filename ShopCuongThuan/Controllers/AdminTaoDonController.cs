using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminTaoDonController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminTaoDonController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var sanPhams = _context.SanPham
                .Include(sp => sp.DanhMuc)
                .OrderBy(sp => sp.TenSP)
                .ToList();

            return View(sanPhams);
        }

        [HttpPost]
        public IActionResult CreateDirectOrder(
            string hoTenNhan,
            string soDienThoai,
            string diaChi,
            string phuongThucThanhToan,
            List<int> selectedProducts)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;
            // Kiểm tra số điện thoại nếu khách có nhập
            if (!string.IsNullOrWhiteSpace(soDienThoai))
            {
                soDienThoai = soDienThoai.Trim();

                if (!System.Text.RegularExpressions.Regex.IsMatch(
                    soDienThoai, @"^0\d{9}$"))
                {
                    TempData["Error"] =
                        "Số điện thoại không hợp lệ.";

                    return RedirectToAction("Index");
                }
            }

            if (selectedProducts == null || selectedProducts.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất 1 sản phẩm.";
                return RedirectToAction("Index");
            }

            var khachVangLai = _context.KhachHang
                .FirstOrDefault(kh => kh.Email == "khachvanglai@cuongthuan.local");

            if (khachVangLai == null)
            {
                khachVangLai = new KhachHang
                {
                    HoTen = "Khách vãng lai",
                    Email = "khachvanglai@cuongthuan.local",
                    SoDienThoai = "0000000000",
                    DiaChi = "Mua trực tiếp tại cửa hàng",
                    MatKhau = "123456",
                    NgayTao = DateTime.Now
                };

                _context.KhachHang.Add(khachVangLai);
                _context.SaveChanges();
            }

            var sanPhams = _context.SanPham
                .Where(sp => selectedProducts.Contains(sp.MaSP))
                .ToList();

            decimal tongTien = 0;

            var chiTietTam = new List<ChiTietDonHang>();

            foreach (var sp in sanPhams)
            {
                int soLuong = 0;

                var qtyValue = Request.Form["qty_" + sp.MaSP];

                if (!string.IsNullOrWhiteSpace(qtyValue))
                {
                    int.TryParse(qtyValue, out soLuong);
                }

                if (soLuong <= 0)
                {
                    continue;
                }

                if (soLuong > sp.SoLuongTon)
                {
                    TempData["Error"] = $"Sản phẩm {sp.TenSP} không đủ tồn kho.";
                    return RedirectToAction("Index");
                }

                var thanhTien = sp.GiaBan * soLuong;
                tongTien += thanhTien;

                chiTietTam.Add(new ChiTietDonHang
                {
                    MaSP = sp.MaSP,
                    SoLuong = soLuong,
                    DonGia = sp.GiaBan,
                    ThanhTien = thanhTien
                });
            }

            if (chiTietTam.Count == 0)
            {
                TempData["Error"] = "Vui lòng nhập số lượng sản phẩm cần bán.";
                return RedirectToAction("Index");
            }

            var donHang = new DonHang
            {
                MaKH = khachVangLai.MaKH,
                MaTraCuu = "TT" + DateTime.Now.ToString("yyyyMMddHHmmssfff"),
                NgayDat = DateTime.Now,
                HoTenNhan = string.IsNullOrWhiteSpace(hoTenNhan) ? "Khách mua trực tiếp" : hoTenNhan,
                SoDienThoai = string.IsNullOrWhiteSpace(soDienThoai) ? "Không có" : soDienThoai,
                DiaChi = string.IsNullOrWhiteSpace(diaChi) ? "Mua trực tiếp tại cửa hàng" : diaChi,
                TongTien = tongTien,
                TrangThai = "Hoàn thành",
                PhuongThucThanhToan = phuongThucThanhToan
            };

            _context.DonHang.Add(donHang);
            _context.SaveChanges();

            foreach (var ct in chiTietTam)
            {
                ct.MaDH = donHang.MaDH;
                _context.ChiTietDonHang.Add(ct);

                var sp = sanPhams.First(x => x.MaSP == ct.MaSP);
                sp.SoLuongTon -= ct.SoLuong;

                if (sp.SoLuongTon < 0)
                {
                    sp.SoLuongTon = 0;
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Tạo đơn bán trực tiếp thành công.";

            return RedirectToAction("Details", "AdminDonHang", new { id = donHang.MaDH });
        }
    }
}