using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;

namespace QuanLyCLB_LSC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly QlClbLscContext _context;

        public RegisterController(QlClbLscContext context)
        {
            _context = context;
        }

        // ===== ĐĂNG KÝ =====
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Register/Register.cshtml");
        }

        [HttpPost]
        public IActionResult Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View("~/Views/Register/Register.cshtml", model);

            if (_context.TaiKhoans.Any(t => t.TenDn == model.TenDN))
            {
                ViewBag.Error = "Tên đăng nhập đã tồn tại!";
                return View("~/Views/Register/Register.cshtml", model);
            }

            var taiKhoan = new TaiKhoan
            {
                TenDn = model.TenDN,
                MatKhau = model.MatKhau,
                NgayTao = DateTime.Now,
                TrangThai = "Hoạt động",
                QuyenHan = "Member"
            };

            _context.TaiKhoans.Add(taiKhoan);
            _context.SaveChanges();

            TempData["Success"] = "Đăng ký thành công! Mời bạn đăng nhập.";
            return RedirectToAction("Login", "Auth");
        }
    }
}
