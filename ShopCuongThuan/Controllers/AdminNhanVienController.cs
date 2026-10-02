using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;

namespace ShopCuongThuan.Controllers
{
    public class AdminNhanVienController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminNhanVienController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var data = _context.NhanVien
                .Include(nv => nv.PhanQuyen)
                .ToList();

            return View(data);
        }

        public IActionResult Create()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            ViewBag.PhanQuyen = _context.PhanQuyen.ToList();
            return View();
        }

        [HttpPost]
        public IActionResult Create(NhanVien model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            _context.NhanVien.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var nv = _context.NhanVien.Find(id);

            if (nv == null)
            {
                return RedirectToAction("Index");
            }

            ViewBag.PhanQuyen = _context.PhanQuyen.ToList();
            return View(nv);
        }

        [HttpPost]
        public IActionResult Edit(NhanVien model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            if (!ModelState.IsValid)
            {
                ViewBag.PhanQuyen = _context.PhanQuyen.ToList();
                return View(model);
            }

            var nv = _context.NhanVien.Find(model.MaNV);

            if (nv == null)
            {
                return RedirectToAction("Index");
            }

            nv.TenNV = model.TenNV;
            nv.Email = model.Email;
            nv.SoDienThoai = model.SoDienThoai;
            nv.Username = model.Username;
            nv.Password = model.Password;
            nv.MaQuyen = model.MaQuyen;
            nv.TrangThai = model.TrangThai;

            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var nv = _context.NhanVien.Find(id);

            if (nv != null)
            {
                _context.NhanVien.Remove(nv);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}