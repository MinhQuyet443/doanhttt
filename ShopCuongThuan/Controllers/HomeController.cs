using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search, string? hang, int? madm)
        {
            var today = DateTime.Today;

            var query = _context.SanPham
                .Include(sp => sp.DanhMuc)
                .Include(sp => sp.KhuyenMais)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(sp => sp.TenSP.Contains(search));
            }

            if (!string.IsNullOrWhiteSpace(hang))
            {
                query = query.Where(sp => sp.Hang == hang);
            }

            if (madm.HasValue)
            {
                query = query.Where(sp => sp.MaDM == madm.Value);
            }

           var products = query
                .ToList()
                .Select(sp =>
                {
                    var khuyenMai = sp.KhuyenMais
                        .FirstOrDefault(km =>
                            km.TrangThai == true &&
                            (km.NgayBatDau == null || km.NgayBatDau <= today) &&
                            (km.NgayKetThuc == null || km.NgayKetThuc >= today));

                    decimal giaHienThi = sp.GiaBan;
                    bool coKhuyenMai = false;
                    string? giamGiaText = null;
                    string? tenKhuyenMai = null;

                    if (khuyenMai != null)
                    {
                        coKhuyenMai = true;
                        tenKhuyenMai = khuyenMai.TenKM;

                        if (khuyenMai.LoaiGiamGia == "PhanTram")
                        {
                            giaHienThi = sp.GiaBan - (sp.GiaBan * khuyenMai.GiaTriGiam / 100);
                            giamGiaText = $"Giảm {khuyenMai.GiaTriGiam:N0}%";
                        }
                        else if (khuyenMai.LoaiGiamGia == "SoTien")
                        {
                            giaHienThi = sp.GiaBan - khuyenMai.GiaTriGiam;
                            giamGiaText = $"Giảm {khuyenMai.GiaTriGiam:N0}đ";
                        }

                        if (giaHienThi < 0)
                        {
                            giaHienThi = 0;
                        }
                    }

                    return new SanPhamCardViewModel
                    {
                        MaSP = sp.MaSP,
                        TenSP = sp.TenSP,
                        Hang = sp.Hang,
                        TenDM = sp.DanhMuc?.TenDM,
                        GiaBan = sp.GiaBan,
                        GiaHienThi = giaHienThi,
                        CoKhuyenMai = coKhuyenMai,
                        GiamGiaText = giamGiaText,
                        TenKhuyenMai = tenKhuyenMai,
                        MoTa = sp.MoTa,
                        HinhAnh = sp.HinhAnh,
                        SoLuongTon = sp.SoLuongTon
                    };
                })
                .ToList();

            ViewBag.Search = search;
            ViewBag.Hang = hang;
            ViewBag.MaDM = madm;
            ViewBag.DanhMuc = _context.DanhMuc.ToList();

            return View(products);
        }
    }
}