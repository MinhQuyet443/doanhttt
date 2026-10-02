using Microsoft.AspNetCore.Mvc;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;
using System.Net;
using System.Net.Mail;

namespace ShopCuongThuan.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AccountController(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        private void SendResetCodeEmail(string toEmail, string code)
        {
            var host = _configuration["EmailSettings:Host"];
            var port = int.Parse(_configuration["EmailSettings:Port"]!);
            var email = _configuration["EmailSettings:Email"];
            var password = _configuration["EmailSettings:Password"];
            var enableSsl = bool.Parse(_configuration["EmailSettings:EnableSsl"]!);

            var subject = "Mã xác nhận đặt lại mật khẩu";
            var body = $@"
                <h2>Cường Thuận Shop</h2>
                <p>Xin chào,</p>
                <p>Mã xác nhận đặt lại mật khẩu của bạn là:</p>
                <h1 style='color:red'>{code}</h1>
                <p>Mã này có hiệu lực trong 5 phút.</p>
                <p>Nếu bạn không yêu cầu đặt lại mật khẩu, vui lòng bỏ qua email này.</p>
            ";

            using var smtp = new SmtpClient(host, port);
            smtp.Credentials = new NetworkCredential(email, password);
            smtp.EnableSsl = enableSsl;

            using var message = new MailMessage();
            message.From = new MailAddress(email!, "Cường Thuận Shop");
            message.To.Add(toEmail);
            message.Subject = subject;
            message.Body = body;
            message.IsBodyHtml = true;

            smtp.Send(message);
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Register(string hoTen, string email, string soDienThoai, string diaChi, string matKhau)
        {
            if (string.IsNullOrWhiteSpace(matKhau) || matKhau.Length <= 8)
            {
                ViewBag.Error = "Mật khẩu phải có trên 8 ký tự.";
                return View();
            }
            var checkEmail = _context.KhachHang.FirstOrDefault(kh => kh.Email == email);

            if (checkEmail != null)
            {
                ViewBag.Error = "Email này đã được sử dụng.";
                return View();
            }
            if (string.IsNullOrWhiteSpace(soDienThoai) ||
    !System.Text.RegularExpressions.Regex.IsMatch(
        soDienThoai.Trim(), @"^0\d{9}$"))
            {
                ViewBag.Error = "Số điện thoại không hợp lệ.";
                return View();
            }
            var khachHang = new KhachHang
            {
                HoTen = hoTen,
                Email = email,
                SoDienThoai = soDienThoai,
                DiaChi = diaChi,
                MatKhau = matKhau,
                NgayTao = DateTime.Now
            };

            _context.KhachHang.Add(khachHang);
            _context.SaveChanges();

            TempData["Success"] = "Đăng ký thành công. Vui lòng đăng nhập.";

            return RedirectToAction("Login");
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string taiKhoan, string matKhau)
        {
            var nhanVien = _context.NhanVien
                .FirstOrDefault(nv =>
                    (nv.Email == taiKhoan || nv.SoDienThoai == taiKhoan) &&
                    nv.Password == matKhau &&
                    nv.TrangThai == true);

            if (nhanVien != null)
            {
                HttpContext.Session.SetInt32("MaNV", nhanVien.MaNV);
                HttpContext.Session.SetString("TenNV", nhanVien.TenNV);
                HttpContext.Session.SetInt32("MaQuyen", nhanVien.MaQuyen);
                HttpContext.Session.SetString("LoaiTaiKhoan", "NhanVien");

                return RedirectToAction("Index", "Admin");
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh =>
                    (kh.Email == taiKhoan || kh.SoDienThoai == taiKhoan) &&
                    kh.MatKhau == matKhau);

            if (khachHang != null)
            {
                HttpContext.Session.SetInt32("MaKH", khachHang.MaKH);
                HttpContext.Session.SetString("HoTen", khachHang.HoTen);
                HttpContext.Session.SetString("LoaiTaiKhoan", "KhachHang");

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Email/Số điện thoại hoặc mật khẩu không đúng.";
            return View();
        }

        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        public IActionResult ForgotPassword(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Vui lòng nhập email.";
                return View();
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh => kh.Email == email);

            if (khachHang == null)
            {
                ViewBag.Error = "Email khách hàng không tồn tại trong hệ thống.";
                return View();
            }

            var random = new Random();
            var code = random.Next(100000, 999999).ToString();

            HttpContext.Session.SetString("ResetEmail", email);
            HttpContext.Session.SetString("ResetCode", code);
            HttpContext.Session.SetString("ResetExpire", DateTime.Now.AddMinutes(5).ToString("O"));
            HttpContext.Session.SetString("ResetVerified", "false");

            try
            {
                SendResetCodeEmail(email, code);
                TempData["Success"] = "Mã xác nhận đã được gửi về email của bạn.";
                return RedirectToAction("VerifyCode");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Không gửi được email: " + ex.Message;
                return View();
            }
        }

        public IActionResult VerifyCode()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyCode(string code)
        {
            var resetCode = HttpContext.Session.GetString("ResetCode");
            var expireString = HttpContext.Session.GetString("ResetExpire");

            if (resetCode == null || expireString == null)
            {
                ViewBag.Error = "Mã xác nhận không tồn tại hoặc đã hết hạn.";
                return View();
            }

            var expireTime = DateTime.Parse(expireString);

            if (DateTime.Now > expireTime)
            {
                ViewBag.Error = "Mã xác nhận đã hết hạn.";
                return View();
            }

            if (code != resetCode)
            {
                ViewBag.Error = "Mã xác nhận không đúng.";
                return View();
            }

            HttpContext.Session.SetString("ResetVerified", "true");

            return RedirectToAction("ResetPassword");
        }

        public IActionResult ResetPassword()
        {
            var verified = HttpContext.Session.GetString("ResetVerified");

            if (verified != "true")
            {
                return RedirectToAction("ForgotPassword");
            }

            return View();
        }

        [HttpPost]
        public IActionResult ResetPassword(string newPassword, string confirmPassword)
        {
            var verified = HttpContext.Session.GetString("ResetVerified");
            var email = HttpContext.Session.GetString("ResetEmail");

            if (verified != "true" || email == null)
            {
                return RedirectToAction("ForgotPassword");
            }

            if (string.IsNullOrWhiteSpace(newPassword) || newPassword.Length <= 8)
            {
                ViewBag.Error = "Mật khẩu phải có trên 8 ký tự.";
                return View();
            }

            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh => kh.Email == email);

            if (khachHang == null)
            {
                ViewBag.Error = "Không tìm thấy tài khoản khách hàng.";
                return View();
            }

            khachHang.MatKhau = newPassword;
            _context.SaveChanges();

            HttpContext.Session.Remove("ResetEmail");
            HttpContext.Session.Remove("ResetCode");
            HttpContext.Session.Remove("ResetExpire");
            HttpContext.Session.Remove("ResetVerified");

            TempData["Success"] = "Đặt lại mật khẩu thành công.";

            return RedirectToAction("Login");
        }
        // ================= QUẢN LÝ THÔNG TIN CÁ NHÂN =================

        // Hiển thị thông tin cá nhân
        public IActionResult Profile()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            // Chưa đăng nhập thì quay về trang đăng nhập
            if (maKH == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh => kh.MaKH == maKH.Value);

            if (khachHang == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(khachHang);
        }


        // Hiển thị form cập nhật thông tin
        public IActionResult UpdateProfile()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh => kh.MaKH == maKH.Value);

            if (khachHang == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            return View(khachHang);
        }


        // Xử lý cập nhật thông tin
        [HttpPost]
        public IActionResult UpdateProfile(
            string hoTen,
            string email,
            string? soDienThoai,
            string? diaChi)
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh => kh.MaKH == maKH.Value);

            if (khachHang == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            hoTen = (hoTen ?? "").Trim();
            email = (email ?? "").Trim();
            soDienThoai = soDienThoai?.Trim();
            diaChi = diaChi?.Trim();

            // Kiểm tra dữ liệu
            if (string.IsNullOrWhiteSpace(hoTen) ||
                string.IsNullOrWhiteSpace(email))
            {
                ViewBag.Error = "Họ tên và email không được để trống.";
                return View(khachHang);
            }
            // Kiểm tra định dạng số điện thoại
            if (!string.IsNullOrWhiteSpace(soDienThoai) &&
                !System.Text.RegularExpressions.Regex.IsMatch(
                    soDienThoai, @"^0\d{9}$"))
            {
                ViewBag.Error = "Số điện thoại sai định dạng.";
                return View(khachHang);
            }

            // Kiểm tra email có bị tài khoản khác sử dụng không
            var emailDaTonTai = _context.KhachHang.Any(kh =>
                kh.Email == email &&
                kh.MaKH != maKH.Value);

            if (emailDaTonTai)
            {
                ViewBag.Error = "Email này đã được sử dụng .";
                return View(khachHang);
            }

            // Cập nhật dữ liệu
            khachHang.HoTen = hoTen;
            khachHang.Email = email;
            khachHang.SoDienThoai = soDienThoai;
            khachHang.DiaChi = diaChi;

            _context.SaveChanges();

            // Cập nhật lại tên hiển thị trên thanh header
            HttpContext.Session.SetString("HoTen", khachHang.HoTen);

            TempData["Success"] = "Cập nhật thông tin cá nhân thành công.";

            return RedirectToAction("Profile");
        }


        // Hiển thị trang đổi mật khẩu
        public IActionResult ChangePassword()
        {
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
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
            var maKH = HttpContext.Session.GetInt32("MaKH");

            if (maKH == null)
            {
                return RedirectToAction("Login");
            }

            var khachHang = _context.KhachHang
                .FirstOrDefault(kh => kh.MaKH == maKH.Value);

            if (khachHang == null)
            {
                HttpContext.Session.Clear();
                return RedirectToAction("Login");
            }

            // Kiểm tra mật khẩu hiện tại
            if (khachHang.MatKhau != currentPassword)
            {
                ViewBag.Error = "Mật khẩu hiện tại không đúng.";
                return View();
            }

            // Mật khẩu mới tối thiểu 6 ký tự
            // Mật khẩu mới phải trên 8 ký tự
            if (string.IsNullOrWhiteSpace(newPassword) ||
                newPassword.Length <= 8)
            {
                ViewBag.Error = "Mật khẩu mới phải có trên 8 ký tự.";
                return View();
            }

            // Kiểm tra xác nhận mật khẩu
            if (newPassword != confirmPassword)
            {
                ViewBag.Error = "Mật khẩu xác nhận không khớp.";
                return View();
            }

            // Không cho đặt giống mật khẩu hiện tại
            if (newPassword == currentPassword)
            {
                ViewBag.Error = "Mật khẩu mới phải khác mật khẩu hiện tại.";
                return View();
            }

            khachHang.MatKhau = newPassword;

            _context.SaveChanges();

            TempData["Success"] = "Đổi mật khẩu thành công.";

            return RedirectToAction("Profile");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index", "Home");
        }
    }
}