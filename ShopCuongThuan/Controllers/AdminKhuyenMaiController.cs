using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminKhuyenMaiController : AdminBaseController
    {
        private readonly AppDbContext _context;

        private bool KiemTraKhuyenMai(KhuyenMai model)
        {
            if (model.GiaTriGiam < 0)
            {
                ModelState.AddModelError(
                    "GiaTriGiam",
                    "Giá trị giảm không được âm");
            }

            if (model.LoaiGiamGia == "PhanTram" &&
                model.GiaTriGiam > 100)
            {
                ModelState.AddModelError(
                    "GiaTriGiam",
                    "Phần trăm giảm không được vượt quá 100%");
            }

            var homNay = DateTime.Today;

            if (!model.NgayBatDau.HasValue ||
                !model.NgayKetThuc.HasValue)
            {
                ModelState.AddModelError(
                    "",
                    "Vui lòng nhập đầy đủ ngày bắt đầu và ngày kết thúc");
            }
            else
            {
                if (model.NgayKetThuc.Value.Date <=
                    model.NgayBatDau.Value.Date)
                {
                    ModelState.AddModelError(
                        "NgayKetThuc",
                        "Ngày kết thúc phải sau ngày bắt đầu");
                }

                if (model.NgayKetThuc.Value.Date <= homNay)
                {
                    ModelState.AddModelError(
                        "NgayKetThuc",
                        "Ngày kết thúc phải sau ngày hôm nay");
                }
            }

            return ModelState.IsValid;
        }

        public AdminKhuyenMaiController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var homNay = DateTime.Today;

            var khuyenMaiHetHan = _context.KhuyenMai
                .Where(km => km.TrangThai &&
                            km.NgayKetThuc.HasValue &&
                            km.NgayKetThuc.Value.Date < homNay)
                .ToList();

            foreach (var km in khuyenMaiHetHan)
            {
                km.TrangThai = false;
            }

            if (khuyenMaiHetHan.Count > 0)
            {
                _context.SaveChanges();
            }
            var query = _context.KhuyenMai
                .Include(km => km.SanPham)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(km =>
                    km.TenKM.Contains(search) ||
                    km.LoaiGiamGia.Contains(search) ||
                    km.SanPham!.TenSP.Contains(search));
            }

            ViewBag.Search = search;

            var khuyenMais = query.ToList();

            return View(khuyenMais);
        }

        public IActionResult Create()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            ViewBag.SanPham = _context.SanPham.ToList();
            return View();
        }


        [HttpPost]
        public IActionResult Create(KhuyenMai model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            if (!KiemTraKhuyenMai(model))
            {
                ViewBag.SanPham = _context.SanPham.ToList();
                return View(model);
            }

          

            _context.KhuyenMai.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var km = _context.KhuyenMai.Find(id);

            if (km == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.SanPham = _context.SanPham.ToList();
            return View(km);
        }


        [HttpPost]
        public IActionResult Edit(KhuyenMai model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            if (!KiemTraKhuyenMai(model))
            {
                ViewBag.SanPham = _context.SanPham.ToList();
                return View(model);
            }

            var km = _context.KhuyenMai.Find(model.MaKM);
            if (km == null) return RedirectToAction("Index");

            km.MaSP = model.MaSP;
            km.TenKM = model.TenKM;
            km.LoaiGiamGia = model.LoaiGiamGia;
            km.GiaTriGiam = model.GiaTriGiam;
            km.NgayBatDau = model.NgayBatDau;
            km.NgayKetThuc = model.NgayKetThuc;
            

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var km = _context.KhuyenMai.Find(id);

            if (km != null)
            {
                _context.KhuyenMai.Remove(km);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}