using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class CartController : Controller
    {
        private readonly AppDbContext _context;

        public CartController(AppDbContext context)
        {
            _context = context;
        }

        private int? GetMaKH()
        {
            return HttpContext.Session.GetInt32("MaKH");
        }

        private decimal TinhGiaSauGiam(SanPham sp, out bool coKhuyenMai, out string? giamGiaText)
        {
            var today = DateTime.Today;

            var khuyenMai = sp.KhuyenMais.FirstOrDefault(km =>
                km.TrangThai == true &&
                (km.NgayBatDau == null || km.NgayBatDau <= today) &&
                (km.NgayKetThuc == null || km.NgayKetThuc >= today));

            coKhuyenMai = false;
            giamGiaText = null;

            var gia = sp.GiaBan;

            if (khuyenMai != null)
            {
                coKhuyenMai = true;

                if (khuyenMai.LoaiGiamGia == "PhanTram")
                {
                    gia = sp.GiaBan - (sp.GiaBan * khuyenMai.GiaTriGiam / 100);
                    giamGiaText = $"Giảm {khuyenMai.GiaTriGiam:N0}%";
                }
                else if (khuyenMai.LoaiGiamGia == "SoTien")
                {
                    gia = sp.GiaBan - khuyenMai.GiaTriGiam;
                    giamGiaText = $"Giảm {khuyenMai.GiaTriGiam:N0}đ";
                }

                if (gia < 0)
                {
                    gia = 0;
                }
            }

            return gia;
        }

        public IActionResult Index()
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

            var model = cartItems.Select(gh =>
            {
                var sp = gh.SanPham!;

                bool coKhuyenMai;
                string? giamGiaText;

                var donGia = TinhGiaSauGiam(sp, out coKhuyenMai, out giamGiaText);

                return new GioHangItemViewModel
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    HinhAnh = sp.HinhAnh,
                    GiaGoc = sp.GiaBan,
                    DonGia = donGia,
                    CoKhuyenMai = coKhuyenMai,
                    GiamGiaText = giamGiaText,
                    SoLuong = gh.SoLuong,
                    ThanhTien = donGia * gh.SoLuong
                };
            }).ToList();

            return View(model);
        }

        [HttpPost]
        public IActionResult Add(int id)
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var sanPham = _context.SanPham.FirstOrDefault(sp => sp.MaSP == id);

            if (sanPham == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var gioHang = _context.GioHang
                .FirstOrDefault(gh => gh.MaKH == maKH.Value && gh.MaSP == id);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    MaKH = maKH.Value,
                    MaSP = id,
                    SoLuong = 1
                };

                _context.GioHang.Add(gioHang);
            }
            else
            {
                gioHang.SoLuong += 1;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Update(int maSP, int soLuong)
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (soLuong < 1)
            {
                soLuong = 1;
            }

            var gioHang = _context.GioHang
                .FirstOrDefault(gh => gh.MaKH == maKH.Value && gh.MaSP == maSP);

            if (gioHang != null)
            {
                gioHang.SoLuong = soLuong;
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int maSP)
        {
            var maKH = GetMaKH();

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var gioHang = _context.GioHang
                .FirstOrDefault(gh => gh.MaKH == maKH.Value && gh.MaSP == maSP);

            if (gioHang != null)
            {
                _context.GioHang.Remove(gioHang);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult BuyNow(int id)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var sanPham = _context.SanPham.FirstOrDefault(sp => sp.MaSP == id);

            if (sanPham == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var gioHang = _context.GioHang
                .FirstOrDefault(gh => gh.MaKH == maKH.Value && gh.MaSP == id);

            if (gioHang == null)
            {
                gioHang = new GioHang
                {
                    MaKH = maKH.Value,
                    MaSP = id,
                    SoLuong = 1
                };

                _context.GioHang.Add(gioHang);
            }
            else
            {
                gioHang.SoLuong += 1;
            }

            _context.SaveChanges();

            return RedirectToAction("Checkout", "Order");
        }
    }
}