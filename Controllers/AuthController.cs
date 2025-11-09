using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;

namespace QuanLyCLB_LSC.Controllers
{
    public class AuthController : Controller
    {
        private readonly QlClbLscContext _context;

        public AuthController(QlClbLscContext context)
        {
            _context = context;
        }

        // ===== ĐĂNG NHẬP =====
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

            if (user.MatKhau != model.MatKhau)
            {
                ViewBag.Error = "Mật khẩu không đúng!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            TempData["LoginSuccess"] = $"Xin chào {user.TenDn}!";
            return RedirectToAction("Index", "Dashboard");
        }

        // ===== ĐĂNG XUẤT =====
        [HttpGet]
        public IActionResult Logout()
        {
            return RedirectToAction("Login");
        }
    }
}
