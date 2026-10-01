using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminNhapHangController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminNhapHangController(AppDbContext context)
        {
            _context = context;
        }

        // Danh sách phiếu nhập
        public IActionResult Index(string? search)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var query = _context.PhieuNhap
                .Include(pn => pn.NhanVien)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(pn =>
                    pn.MaPhieuNhap.Contains(search) ||
                    pn.GhiChu!.Contains(search) ||
                    pn.NhanVien!.TenNV.Contains(search));
            }

            ViewBag.Search = search;

            var phieuNhaps = query
                .OrderByDescending(pn => pn.NgayNhap)
                .ToList();

            return View(phieuNhaps);
        }

     
        public IActionResult Create()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var sanPhams = _context.SanPham
                .Include(sp => sp.DanhMuc)
                .OrderBy(sp => sp.TenSP)
                .ToList();

            return View(sanPhams);
        }

        // Xử lý tạo phiếu nhập
        [HttpPost]
        public IActionResult Create(string maPhieuNhap, List<int> selectedProducts, string? ghiChu)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            if (string.IsNullOrWhiteSpace(maPhieuNhap))
            {
                TempData["Error"] = "Vui lòng nhập mã phiếu nhập.";
                return RedirectToAction("Create");
            }

            maPhieuNhap = maPhieuNhap.Trim();

            var checkMaPhieu = _context.PhieuNhap
                .FirstOrDefault(pn => pn.MaPhieuNhap == maPhieuNhap);

            if (checkMaPhieu != null)
            {
                TempData["Error"] = "Mã phiếu nhập đã tồn tại.";
                return RedirectToAction("Create");
            }

            if (selectedProducts == null || selectedProducts.Count == 0)
            {
                TempData["Error"] = "Vui lòng chọn ít nhất 1 sản phẩm để nhập hàng.";
                return RedirectToAction("Create");
            }

            var maNV = HttpContext.Session.GetInt32("MaNV");

            var sanPhams = _context.SanPham
                .Where(sp => selectedProducts.Contains(sp.MaSP))
                .ToList();

            var chiTietTam = new List<ChiTietPhieuNhap>();
            decimal tongTien = 0;

            foreach (var sp in sanPhams)
            {
                int soLuongNhap = 0;
                decimal donGiaNhap = 0;

                var qtyValue = Request.Form["qty_" + sp.MaSP];
                var priceValue = Request.Form["price_" + sp.MaSP];

                if (!string.IsNullOrWhiteSpace(qtyValue))
                {
                    int.TryParse(qtyValue, out soLuongNhap);
                }

                if (!string.IsNullOrWhiteSpace(priceValue))
                {
                    decimal.TryParse(priceValue, out donGiaNhap);
                }

                if (soLuongNhap <= 0)
                {
                    continue;
                }

                if (donGiaNhap < 0)
                {
                    donGiaNhap = 0;
                }

                var thanhTien = soLuongNhap * donGiaNhap;
                tongTien += thanhTien;

                chiTietTam.Add(new ChiTietPhieuNhap
                {
                    MaSP = sp.MaSP,
                    SoLuongNhap = soLuongNhap,
                    DonGiaNhap = donGiaNhap,
                    ThanhTien = thanhTien
                });
            }

            if (chiTietTam.Count == 0)
            {
                TempData["Error"] = "Vui lòng nhập số lượng nhập hợp lệ.";
                return RedirectToAction("Create");
            }

            var phieuNhap = new PhieuNhap
            {
                MaPhieuNhap = maPhieuNhap,
                NgayNhap = DateTime.Now,
                MaNV = maNV,
                TongTien = tongTien,
                GhiChu = ghiChu
            };

            _context.PhieuNhap.Add(phieuNhap);
            _context.SaveChanges();

            foreach (var ct in chiTietTam)
            {
                ct.MaPN = phieuNhap.MaPN;
                _context.ChiTietPhieuNhap.Add(ct);

                var sanPham = sanPhams.FirstOrDefault(sp => sp.MaSP == ct.MaSP);

                if (sanPham != null)
                {
                    sanPham.SoLuongTon += ct.SoLuongNhap;
                }
            }

            _context.SaveChanges();

            TempData["Success"] = "Nhập hàng thành công. Số lượng tồn kho đã được cập nhật.";

            return RedirectToAction("Details", new { id = phieuNhap.MaPN });
        }

        // Chi tiết phiếu nhập
        public IActionResult Details(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var phieuNhap = _context.PhieuNhap
                .Include(pn => pn.NhanVien)
                .Include(pn => pn.ChiTietPhieuNhaps)
                    .ThenInclude(ct => ct.SanPham)
                .FirstOrDefault(pn => pn.MaPN == id);

            if (phieuNhap == null)
            {
                return RedirectToAction("Index");
            }

            return View(phieuNhap);
        }
    }
}