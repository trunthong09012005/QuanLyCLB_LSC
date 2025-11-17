using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using QuanLyCLB_LSC.Helpers;

namespace QuanLyCLB_LSC.Controllers
{
    public class RegisterController : Controller
    {
        private readonly QlClbLscContext _context;

        public RegisterController(QlClbLscContext context)
        {
            _context = context;
        }

        // ===== ĐĂNG KÝ - GET =====
        [HttpGet]
        public IActionResult Register()
        {
            return View("~/Views/Register/Register.cshtml");
        }

        // ===== ĐĂNG KÝ - POST =====
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel model)
        {
            // Kiểm tra form
            if (!ModelState.IsValid)
                return View("~/Views/Register/Register.cshtml", model);

            // 1️⃣ Kiểm tra tên đăng nhập
            if (_context.TaiKhoans.Any(t => t.TenDn.ToLower() == model.TenDN.ToLower()))
            {
                ModelState.AddModelError(nameof(model.TenDN), "Tên đăng nhập đã tồn tại!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2️⃣ Kiểm tra email
            var email = model.Email?.Trim().ToLower();
            if (!string.IsNullOrWhiteSpace(email) &&
                _context.ThanhViens.Any(tv => tv.Email.ToLower() == email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // 3️⃣ Tạo ThanhVien
                var thanhVien = new ThanhVien
                {
                    HoTen = string.IsNullOrWhiteSpace(model.HoTen) ? model.TenDN : model.HoTen,
                    NgaySinh = model.NgaySinh.HasValue ? DateOnly.FromDateTime(model.NgaySinh.Value) : null,
                    GioiTinh = model.GioiTinh,
                    Sdt = model.SDT,
                    DiaChi = model.DiaChi,
                    Khoa = model.Khoa,
                    Lop = model.Lop,
                    Email = model.Email,
                    VaiTro = model.VaiTro,
                    NgayThamGia = DateOnly.FromDateTime(DateTime.Now),
                    TrangThai = "Hoạt động"
                };

                _context.ThanhViens.Add(thanhVien);
                _context.SaveChanges(); // EF Core gán MaTv tự động
                //transaction.Commit();

                // 4️⃣ Tạo TaiKhoan với MaTv
                var taiKhoan = new TaiKhoan
                {
                    TenDn = model.TenDN,
                    MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                    NgayTao = DateTime.Now,
                    TrangThai = "Hoạt động",
                    QuyenHan = "Thành viên",
                    MaTv = thanhVien.MaTv
                };

                _context.TaiKhoans.Add(taiKhoan);
                _context.SaveChanges();
                transaction.Commit();

                TempData["Success"] = "Đăng ký thành công! Mời bạn đăng nhập.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                // Nếu lỗi, show message
                ModelState.AddModelError("", "Có lỗi khi lưu dữ liệu. Vui lòng thử lại.");
                // Debug: nếu muốn xem lỗi thực tế, bỏ comment dòng dưới
                // ModelState.AddModelError("", ex.Message);
                return View("~/Views/Register/Register.cshtml", model);
            }
        }
    }
}
