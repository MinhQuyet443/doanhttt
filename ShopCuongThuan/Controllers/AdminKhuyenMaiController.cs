using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminKhuyenMaiController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminKhuyenMaiController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(string? search)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

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

            _context.KhuyenMai.Update(model);
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