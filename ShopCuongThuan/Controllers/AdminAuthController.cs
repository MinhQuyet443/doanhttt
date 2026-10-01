using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCuongThuan.Data;

namespace ShopCuongThuan.Controllers
{
    public class AdminAuthController : Controller
    {
        private readonly AppDbContext _context;

        public AdminAuthController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var nhanVien = _context.NhanVien
                .FirstOrDefault(nv =>
                    nv.Username == username &&
                    nv.Password == password &&
                    nv.TrangThai == true);

            if (nhanVien == null)
            {
                ViewBag.Error = "Tài khoản hoặc mật khẩu không đúng.";
                return View();
            }

            HttpContext.Session.SetInt32("MaNV", nhanVien.MaNV);
            HttpContext.Session.SetString("TenNV", nhanVien.TenNV);
            HttpContext.Session.SetInt32("MaQuyen", nhanVien.MaQuyen);

            return RedirectToAction("Index", "Admin");
        }
        // ================= QUẢN LÝ TÀI KHOẢN =================

        // Xem thông tin cá nhân
        public IActionResult Profile()
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login");
            }

            var nhanVien = _context.NhanVien
                .Include(nv => nv.PhanQuyen)
                .FirstOrDefault(nv => nv.MaNV == maNV.Value);

            if (nhanVien == null)
            {
                return RedirectToAction("Login");
            }

            return View(nhanVien);
        }


        // Hiển thị form cập nhật thông tin
        public IActionResult UpdateProfile()
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login");
            }

            var nhanVien = _context.NhanVien
                .FirstOrDefault(nv => nv.MaNV == maNV.Value);

            if (nhanVien == null)
            {
                return RedirectToAction("Login");
            }

            return View(nhanVien);
        }


        // Xử lý cập nhật thông tin
        [HttpPost]
        public IActionResult UpdateProfile(
            string tenNV,
            string? email,
            string? soDienThoai)
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login");
            }

            var nhanVien = _context.NhanVien
                .FirstOrDefault(nv => nv.MaNV == maNV.Value);

            if (nhanVien == null)
            {
                return RedirectToAction("Login");
            }

            tenNV = (tenNV ?? "").Trim();
            email = email?.Trim();
            soDienThoai = soDienThoai?.Trim();

            if (string.IsNullOrWhiteSpace(tenNV))
            {
                ViewBag.Error = "Tên nhân viên không được để trống.";
                return View(nhanVien);
            }

            // Kiểm tra email có bị trùng không
            if (!string.IsNullOrWhiteSpace(email))
            {
                var emailTonTai = _context.NhanVien.Any(nv =>
                    nv.Email == email &&
                    nv.MaNV != maNV.Value);

                if (emailTonTai)
                {
                    ViewBag.Error = "Email này đã được sử dụng bởi tài khoản khác.";
                    return View(nhanVien);
                }
            }

            nhanVien.TenNV = tenNV;
            nhanVien.Email = email;
            nhanVien.SoDienThoai = soDienThoai;

            _context.SaveChanges();

            // Cập nhật lại tên trên header
            HttpContext.Session.SetString("TenNV", nhanVien.TenNV);

            TempData["Success"] = "Cập nhật thông tin thành công.";

            return RedirectToAction("Profile");
        }


        // Hiển thị trang đổi mật khẩu
        public IActionResult ChangePassword()
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login");
            }

            return View();
        }


        // Xử lý đổi mật khẩu
        [HttpPost]
        public IActionResult ChangePassword(
            string currentPassword,
            string newPassword,
            string confirmPassword)
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login");
            }

            var nhanVien = _context.NhanVien
                .FirstOrDefault(nv => nv.MaNV == maNV.Value);

            if (nhanVien == null)
            {
                return RedirectToAction("Login");
            }

            if (nhanVien.Password != currentPassword)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng.";
                return View();
            }

            if (string.IsNullOrWhiteSpace(newPassword)
                || newPassword.Length < 6)
            {
                ViewBag.Error = "Mật khẩu mới phải có ít nhất 6 ký tự.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Xác nhận mật khẩu không khớp.";
                return View();
            }

            if (newPassword == currentPassword)
            {
                ViewBag.Error = "Mật khẩu mới phải khác mật khẩu hiện tại.";
                return View();
            }

            nhanVien.Password = newPassword;

            _context.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công.";

            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("MaNV");
            HttpContext.Session.Remove("TenNV");
            HttpContext.Session.Remove("MaQuyen");

            return RedirectToAction("Login");
        }
    }
}