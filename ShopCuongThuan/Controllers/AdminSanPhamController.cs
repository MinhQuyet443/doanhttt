using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminSanPhamController : AdminBaseController
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;

        public AdminSanPhamController(AppDbContext context, IWebHostEnvironment environment)
        {
            _context = context;
            _environment = environment;
        }

        public IActionResult Index(string? search)
        {
            var check = CheckQuanLy();
            if (check != null) return check;

            var query = _context.SanPham
                .Include(sp => sp.DanhMuc)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(sp =>
                    sp.TenSP.Contains(search) ||
                    sp.Hang!.Contains(search) ||
                    sp.DanhMuc!.TenDM.Contains(search));
            }

            ViewBag.Search = search;

            var sanPhams = query.ToList();

            return View(sanPhams);
        }

        public IActionResult Create()
        {
            var check = CheckQuanLy();
            if (check != null) return check;

            ViewBag.DanhMuc = _context.DanhMuc.ToList();

            return View();
        }

        [HttpPost]
        public IActionResult Create(SanPham model)
        {
            var check = CheckQuanLy();
            if (check != null) return check;

            if (model.UploadImage != null && model.UploadImage.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(model.UploadImage.FileName);

                string folderPath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.UploadImage.CopyTo(stream);
                }

                model.HinhAnh = fileName;
            }
            else
            {
                model.HinhAnh = "no-image.png";
            }

            _context.SanPham.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var sanPham = _context.SanPham.FirstOrDefault(sp => sp.MaSP == id);

            if (sanPham == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.DanhMuc = _context.DanhMuc.ToList();

            return View(sanPham);
        }

        [HttpPost]
        public IActionResult Edit(SanPham model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var sanPham = _context.SanPham.FirstOrDefault(sp => sp.MaSP == model.MaSP);

            if (sanPham == null)
            {
                return RedirectToAction("Index");
            }

            sanPham.TenSP = model.TenSP;
            sanPham.MaDM = model.MaDM;
            sanPham.Hang = model.Hang;
            sanPham.GiaBan = model.GiaBan;
            sanPham.MoTa = model.MoTa;
            sanPham.SoLuongTon = model.SoLuongTon;

            if (model.UploadImage != null && model.UploadImage.Length > 0)
            {
                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(model.UploadImage.FileName);

                string folderPath = Path.Combine(_environment.WebRootPath, "images");

                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string filePath = Path.Combine(folderPath, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    model.UploadImage.CopyTo(stream);
                }

                sanPham.HinhAnh = fileName;
            }

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var sanPham = _context.SanPham
                .FirstOrDefault(sp => sp.MaSP == id);

            if (sanPham == null)
            {
                return RedirectToAction("Index");
            }

            // Xóa sản phẩm khỏi tất cả giỏ hàng
            var gioHangs = _context.GioHang
                .Where(gh => gh.MaSP == id)
                .ToList();

            if (gioHangs.Any())
            {
                _context.GioHang.RemoveRange(gioHangs);
            }

            // Xóa các khuyến mãi của sản phẩm
            var khuyenMais = _context.KhuyenMai
                .Where(km => km.MaSP == id)
                .ToList();

            if (khuyenMais.Any())
            {
                _context.KhuyenMai.RemoveRange(khuyenMais);
            }

            // ChiTietDonHang và ChiTietPhieuNhap KHÔNG XÓA.
            // MaSP sẽ được chuyển thành NULL nhờ DeleteBehavior.SetNull.

            _context.SanPham.Remove(sanPham);

            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}