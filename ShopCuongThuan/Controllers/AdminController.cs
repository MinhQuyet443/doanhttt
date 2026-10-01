using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;

namespace ShopCuongThuan.Controllers
{
    public class AdminController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            ViewBag.SoSanPham = _context.SanPham.Count();
            ViewBag.SoDanhMuc = _context.DanhMuc.Count();
            ViewBag.SoDonHang = _context.DonHang.Count();
            ViewBag.SoNhanVien = _context.NhanVien.Count();
            ViewBag.SoKhuyenMai = _context.KhuyenMai.Count();

            return View();
        }
    }
}