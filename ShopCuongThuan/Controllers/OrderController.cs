using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class OrderController : Controller
    {
        private readonly AppDbContext _context;

        public OrderController(AppDbContext context)
        {
            _context = context;
        }

        private int? GetMaKH()
        {
            return HttpContext.Session.GetInt32("MaKH");
        }

        private decimal TinhGiaSauGiam(SanPham sp)
        {
            var today = DateTime.Today;

            var khuyenMai = sp.KhuyenMais.FirstOrDefault(km =>
                km.TrangThai == true &&
                (km.NgayBatDau == null || km.NgayBatDau <= today) &&
                (km.NgayKetThuc == null || km.NgayKetThuc >= today));

            decimal gia = sp.GiaBan;

            if (khuyenMai != null)
            {
                if (khuyenMai.LoaiGiamGia == "PhanTram")
                {
                    gia = sp.GiaBan - (sp.GiaBan * khuyenMai.GiaTriGiam / 100);
                }
                else if (khuyenMai.LoaiGiamGia == "SoTien")
                {
                    gia = sp.GiaBan - khuyenMai.GiaTriGiam;
                }

                if (gia < 0)
                {
                    gia = 0;
                }
            }

            return gia;
        }

        // Trang thanh toán
        public IActionResult Checkout()
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cartItems = _context.GioHang
                .Include(gh => gh.SanPham)
                    .ThenInclude(sp => sp!.KhuyenMais)
                .Where(gh => gh.MaKH == maKH.Value)
                .ToList();

            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var model = cartItems.Select(gh =>
            {
                var sp = gh.SanPham!;
                var donGia = TinhGiaSauGiam(sp);

                return new GioHangItemViewModel
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    HinhAnh = sp.HinhAnh,
                    DonGia = donGia,
                    SoLuong = gh.SoLuong,
                    ThanhTien = donGia * gh.SoLuong
                };
            }).ToList();

            return View(model);
        }

        // Xử lý đặt hàng
        [HttpPost]
        public IActionResult PlaceOrder(
            string hoTenNhan,
            string soDienThoai,
            string diaChi,
            string phuongThucThanhToan)
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (string.IsNullOrWhiteSpace(hoTenNhan) ||
                string.IsNullOrWhiteSpace(soDienThoai) ||
                string.IsNullOrWhiteSpace(diaChi) ||
                string.IsNullOrWhiteSpace(phuongThucThanhToan))
            {
                return RedirectToAction("Checkout");
            }

            var cartItems = _context.GioHang
                .Include(gh => gh.SanPham)
                    .ThenInclude(sp => sp!.KhuyenMais)
                .Where(gh => gh.MaKH == maKH.Value)
                .ToList();

            if (cartItems.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            decimal tongTien = 0;

            foreach (var item in cartItems)
            {
                var donGia = TinhGiaSauGiam(item.SanPham!);
                tongTien += donGia * item.SoLuong;
            }

            var maTraCuu = "DH" + DateTime.Now.ToString("yyyyMMddHHmmssfff");

            var donHang = new DonHang
            {
                MaKH = maKH.Value,
                MaTraCuu = maTraCuu,
                NgayDat = DateTime.Now,
                HoTenNhan = hoTenNhan,
                SoDienThoai = soDienThoai,
                DiaChi = diaChi,
                TongTien = tongTien,
                TrangThai = "Chờ xác nhận",
                PhuongThucThanhToan = phuongThucThanhToan
            };

            _context.DonHang.Add(donHang);
            _context.SaveChanges();

            foreach (var item in cartItems)
            {
                var sp = item.SanPham!;
                var donGia = TinhGiaSauGiam(sp);

                var chiTiet = new ChiTietDonHang
                {
                    MaDH = donHang.MaDH,
                    MaSP = sp.MaSP,
                    SoLuong = item.SoLuong,
                    DonGia = donGia,
                    ThanhTien = donGia * item.SoLuong
                };

                _context.ChiTietDonHang.Add(chiTiet);

                sp.SoLuongTon -= item.SoLuong;

                if (sp.SoLuongTon < 0)
                {
                    sp.SoLuongTon = 0;
                }
            }

            _context.GioHang.RemoveRange(cartItems);
            _context.SaveChanges();

            return RedirectToAction("Success", new { id = donHang.MaDH });
        }

        // Trang đặt hàng thành công
        public IActionResult Success(int id)
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = _context.DonHang
                .FirstOrDefault(dh => dh.MaDH == id && dh.MaKH == maKH.Value);

            if (donHang == null)
            {
                return RedirectToAction("Index", "Home");
            }

            return View(donHang);
        }

        // Danh sách đơn hàng của khách hàng đang đăng nhập
        public IActionResult MyOrders()
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHangs = _context.DonHang
                .Where(dh => dh.MaKH == maKH.Value)
                .OrderByDescending(dh => dh.NgayDat)
                .ToList();

            return View(donHangs);
        }

        // Chi tiết từng đơn hàng
        public IActionResult Details(int id)
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var donHang = _context.DonHang
                .Include(dh => dh.ChiTietDonHangs)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefault(dh => dh.MaDH == id && dh.MaKH == maKH.Value);

            if (donHang == null)
            {
                return RedirectToAction("MyOrders");
            }

            return View(donHang);
        }

        // Nếu còn link cũ /Order/Lookup thì tự chuyển sang danh sách đơn hàng
        public IActionResult Lookup()
        {
            return RedirectToAction("MyOrders");
        }
    }
}