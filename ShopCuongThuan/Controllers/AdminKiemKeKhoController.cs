using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminKiemKeKhoController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminKiemKeKhoController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var query = _context.SanPham
                .Include(sp => sp.DanhMuc)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(sp => sp.TenSP.Contains(search));
            }

            var model = query
                .OrderBy(sp => sp.TenSP)
                .Select(sp => new KiemKeKhoViewModel
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    HinhAnh = sp.HinhAnh,
                    DanhMuc = sp.DanhMuc != null ? sp.DanhMuc.TenDM : "",
                    Hang = sp.Hang,
                    GiaBan = sp.GiaBan,
                    TonHeThong = sp.SoLuongTon,
                    TonThucTe = sp.SoLuongTon
                })
                .ToList();

            ViewBag.Search = search;

            return View(model);
        }

        [HttpPost]
        public IActionResult KiemKe(List<KiemKeKhoViewModel> items)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            if (items == null || items.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var ids = items.Select(x => x.MaSP).ToList();

            var sanPhams = _context.SanPham
                .Include(sp => sp.DanhMuc)
                .Where(sp => ids.Contains(sp.MaSP))
                .ToList();

            var ketQua = new List<KiemKeKhoViewModel>();

            foreach (var sp in sanPhams)
            {
                var itemNhap = items.FirstOrDefault(x => x.MaSP == sp.MaSP);

                int tonThucTe = itemNhap?.TonThucTe ?? sp.SoLuongTon;

                ketQua.Add(new KiemKeKhoViewModel
                {
                    MaSP = sp.MaSP,
                    TenSP = sp.TenSP,
                    HinhAnh = sp.HinhAnh,
                    DanhMuc = sp.DanhMuc != null ? sp.DanhMuc.TenDM : "",
                    Hang = sp.Hang,
                    GiaBan = sp.GiaBan,
                    TonHeThong = sp.SoLuongTon,
                    TonThucTe = tonThucTe
                });
            }

            ViewBag.DaKiemKe = true;
            ViewBag.TongLechSoLuong = ketQua.Sum(x => x.ChenhLech);
            ViewBag.TongGiaTriLech = ketQua.Sum(x => x.GiaTriChenhLech);

            return View("Index", ketQua.OrderBy(x => x.ChenhLech).ToList());
        }

        [HttpPost]
        public IActionResult CapNhatTonTheoThucTe(List<KiemKeKhoViewModel> items)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            if (items == null || items.Count == 0)
            {
                return RedirectToAction("Index");
            }

            var ids = items.Select(x => x.MaSP).ToList();

            var sanPhams = _context.SanPham
                .Where(sp => ids.Contains(sp.MaSP))
                .ToList();

            foreach (var sp in sanPhams)
            {
                var itemNhap = items.FirstOrDefault(x => x.MaSP == sp.MaSP);

                if (itemNhap != null && itemNhap.TonThucTe >= 0)
                {
                    sp.SoLuongTon = itemNhap.TonThucTe;
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Đã cập nhật tồn kho hệ thống theo số lượng thực tế.";

            return RedirectToAction("Index");
        }
    }
}