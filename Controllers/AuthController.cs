using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using Microsoft.AspNetCore.Http;
using QuanLyCLB_LSC.Helpers;

namespace QuanLyCLB_LSC.Controllers
{
    public class AuthController : Controller
    {
        private readonly QlClbLscContext _context;

        public AuthController(QlClbLscContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        public IActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Auth/Login.cshtml", model);

            var user = _context.TaiKhoans.FirstOrDefault(u => u.TenDn == model.TenDN);

            if (user == null)
            {
                ViewBag.Error = "Tên đăng nhập không tồn tại!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            var storedHash = user.MatKhau ?? string.Empty;

            // Hash input using SHA256 and compare with stored hash
            var inputHash = PasswordHelper.HashPassword(model.MatKhau);
            if (!string.Equals(inputHash, storedHash, System.StringComparison.OrdinalIgnoreCase))
            {
                ViewBag.Error = "Mật khẩu không đúng!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            // Lưu session
            HttpContext.Session.SetInt32("MaTV", user.MaTv);
            HttpContext.Session.SetString("TenDN", user.TenDn);
            HttpContext.Session.SetString("QuyenHan", user.QuyenHan);

            // Redirect theo role
            if (user.QuyenHan == "Quản trị viên" || user.QuyenHan == "Admin")
            {
                return RedirectToAction("Index", "Dashboard");
            }
            else if (user.QuyenHan == "Member" || user.QuyenHan == "Thành viên")
            {
                return RedirectToAction("User", "UserDashboard");

            }
            else
            {
                ViewBag.Error = "Role không hợp lệ!";
                return View("~/Views/Auth/Login.cshtml", model);
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
