using Microsoft.AspNetCore.Mvc;
using ShopCuongThuan.Data;
using ShopCuongThuan.Models;
using System.Text.RegularExpressions;
namespace ShopCuongThuan.Controllers
{
    public class AdminDanhMucController : AdminBaseController
    {
        private readonly AppDbContext _context;

        public AdminDanhMucController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var data = _context.DanhMuc.ToList();
            return View(data);
        }

        public IActionResult Create()
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            return View();
        }

        [HttpPost]
        public IActionResult Create(DanhMuc model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            // Xóa khoảng trắng đầu và cuối
            model.TenDM = (model.TenDM ?? "").Trim();

            // 1. Không được để trống
            if (string.IsNullOrWhiteSpace(model.TenDM))
            {
                ViewBag.Error = "Tên danh mục không được để trống.";
                return View(model);
            }

            // 2. Không được chỉ toàn số
            if (model.TenDM.All(char.IsDigit))
            {
                ViewBag.Error = "Tên danh mục không được chỉ chứa toàn số.";
                return View(model);
            }

            // 3. Chỉ cho phép chữ, số và khoảng trắng
            // \p{L}: tất cả chữ cái, bao gồm tiếng Việt
            // \p{N}: chữ số
            if (!Regex.IsMatch(model.TenDM, @"^[\p{L}\p{N}\s]+$"))
            {
                ViewBag.Error = "Tên danh mục không được chứa ký tự đặc biệt.";
                return View(model);
            }

            // 4. Kiểm tra trùng tên danh mục
            bool daTonTai = _context.DanhMuc
                .Any(x => x.TenDM.ToLower() == model.TenDM.ToLower());

            if (daTonTai)
            {
                ViewBag.Error = "Tên danh mục đã tồn tại.";
                return View(model);
            }

            _context.DanhMuc.Add(model);
            _context.SaveChanges();

            TempData["Success"] = "Thêm danh mục thành công.";

            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var dm = _context.DanhMuc.Find(id);

            if (dm == null)
            {
                return RedirectToAction("Index");
            }

            return View(dm);
        }

        [HttpPost]
        public IActionResult Edit(DanhMuc model)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            model.TenDM = (model.TenDM ?? "").Trim();

            // Không được để trống
            if (string.IsNullOrWhiteSpace(model.TenDM))
            {
                ViewBag.Error = "Tên danh mục không được để trống.";
                return View(model);
            }

            // Không được toàn số
            if (model.TenDM.All(char.IsDigit))
            {
                ViewBag.Error = "Tên danh mục không được chỉ chứa toàn số.";
                return View(model);
            }

            // Không được có ký tự đặc biệt
            if (!Regex.IsMatch(model.TenDM, @"^[\p{L}\p{N}\s]+$"))
            {
                ViewBag.Error = "Tên danh mục không được chứa ký tự đặc biệt.";
                return View(model);
            }

            // Kiểm tra trùng với danh mục KHÁC
            bool daTonTai = _context.DanhMuc.Any(x =>
                x.MaDM != model.MaDM &&
                x.TenDM.ToLower() == model.TenDM.ToLower());

            if (daTonTai)
            {
                ViewBag.Error = "Tên danh mục đã tồn tại.";
                return View(model);
            }

            _context.DanhMuc.Update(model);
            _context.SaveChanges();

            TempData["Success"] = "Cập nhật danh mục thành công.";

            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult Delete(int id)
        {
            var check = CheckAdminLogin();
            if (check != null) return check;

            var dm = _context.DanhMuc.Find(id);

            if (dm != null)
            {
                _context.DanhMuc.Remove(dm);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}