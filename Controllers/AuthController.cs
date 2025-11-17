using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using Microsoft.AspNetCore.Http;

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

            // So sánh mật khẩu plain text (hoặc hash nếu DB hash)
            if (user.MatKhau != model.MatKhau)
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
            else if (user.QuyenHan == "Thành viên")
            {
                return RedirectToAction("User", "UserDashboard", new { maTV = user.MaTv });
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
