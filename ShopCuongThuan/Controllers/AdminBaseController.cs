using Microsoft.AspNetCore.Mvc;

namespace ShopCuongThuan.Controllers
{
    public class AdminBaseController : Controller
    {
        protected IActionResult? CheckAdminLogin()
        {
            var maNV = HttpContext.Session.GetInt32("MaNV");

            if (maNV == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return null;
        }

        protected IActionResult? CheckQuanLy()
        {
            var checkLogin = CheckAdminLogin();

            if (checkLogin != null)
            {
                return checkLogin;
            }

            var maQuyen = HttpContext.Session.GetInt32("MaQuyen");

            // Giả sử MaQuyen = 1 là Quản lý
            if (maQuyen != 1)
            {
                TempData["Error"] = "Bạn không có quyền sử dụng chức năng này.";
                return RedirectToAction("Index", "Admin");
            }

            return null;
        }
    }
}