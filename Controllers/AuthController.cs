using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using Microsoft.AspNetCore.Http;
using QuanLyCLB_LSC.Helpers;
using QuanLyCLB_LSC.Services;
using System.Threading.Tasks;

namespace QuanLyCLB_LSC.Controllers
{
    public class AuthController : Controller
    {
        private readonly QlClbLscContext _context;
        private readonly IAuditService _audit;

        public AuthController(QlClbLscContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Auth/Login.cshtml", model);

            var user = _context.TaiKhoans.FirstOrDefault(u => u.TenDn == model.TenDN);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập không tồn tại!";
                // audit failed login (unknown user)
                await _audit.LogAsync(null, "TaiKhoan", "Đăng nhập thất bại", $"TenDN={model.TenDN}", "Tên đăng nhập không tồn tại");
                return View("~/Views/Auth/Login.cshtml", model);
            }

            var storedHash = user.MatKhau ?? string.Empty;

            // Hash input using SHA256 and compare with stored hash
            var inputHash = PasswordHelper.HashPassword(model.MatKhau);
            if (!string.Equals(inputHash, storedHash, System.StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Mật khẩu không đúng!";
                // audit failed login (wrong password)
                await _audit.LogAsync(user.MaTv, "TaiKhoan", "Đăng nhập thất bại", $"MaTV={user.MaTv}", "Mật khẩu không đúng");
                return View("~/Views/Auth/Login.cshtml", model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("MaTV", user.MaTv);
            HttpContext.Session.SetString("TenDN", user.TenDn);
            HttpContext.Session.SetString("QuyenHan", user.QuyenHan);

            // audit successful login
            await _audit.LogAsync(user.MaTv, "TaiKhoan", "Đăng nhập", $"MaTV={user.MaTv}", $"Đăng nhập: {user.TenDn}");

            // If AJAX request, return JSON so client can redirect without a full form submission
            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

            // Redirect theo role
            if (user.QuyenHan == "Quản trị viên" || user.QuyenHan == "Admin")
            {
                if (isAjax) return Json(new { success = true, redirect = Url.Action("Index", "Dashboard") });
                return RedirectToAction("Index", "Dashboard");
            }
            else if (user.QuyenHan == "Member" || user.QuyenHan == "Thành viên")
            {
                if (isAjax) return Json(new { success = true, redirect = Url.Action("User", "UserDashboard") });
                return RedirectToAction("User", "UserDashboard");

            }
            else
            {
                if (isAjax) return Json(new { success = false, message = "Role không hợp lệ!" });
                ViewBag.Error = "Role không hợp lệ!";
                return View("~/Views/Auth/Login.cshtml", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            // attempt to log logout with current session user if present
            int? maTv = HttpContext.Session.GetInt32("MaTV");
            if (maTv.HasValue)
            {
                await _audit.LogAsync(maTv, "TaiKhoan", "Đăng xuất", $"MaTV={maTv}", "Đăng xuất");
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
