using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using QuanLyCLB_LSC.Helpers;
using System.Text.RegularExpressions;

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
            // ===== 1. KIỂM TRA MODEL STATE =====
            if (!ModelState.IsValid)
            {
                return View("~/Views/Register/Register.cshtml", model);
            }

            // ===== 2. VALIDATION BỔ SUNG =====

            // 2.1. Sanitize inputs (loại bỏ khoảng trắng thừa)
            model.TenDN = model.TenDN?.Trim();
            model.HoTen = model.HoTen?.Trim();
            model.Email = model.Email?.Trim().ToLower();
            model.SDT = model.SDT?.Trim();
            model.DiaChi = model.DiaChi?.Trim();
            model.Khoa = model.Khoa?.Trim();
            model.Lop = model.Lop?.Trim();

            // 2.2. Kiểm tra tên đăng nhập đã tồn tại
            if (_context.TaiKhoans.Any(t => t.TenDn.ToLower() == model.TenDN.ToLower()))
            {
                ModelState.AddModelError(nameof(model.TenDN), "Tên đăng nhập đã tồn tại trong hệ thống!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2.3. Kiểm tra email đã tồn tại
            if (_context.ThanhViens.Any(tv => tv.Email.ToLower() == model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email đã được sử dụng bởi tài khoản khác!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2.4. Kiểm tra số điện thoại đã tồn tại
            if (!string.IsNullOrWhiteSpace(model.SDT) &&
                _context.ThanhViens.Any(tv => tv.Sdt == model.SDT))
            {
                ModelState.AddModelError(nameof(model.SDT), "Số điện thoại đã được sử dụng!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2.5. Validate tên đăng nhập không chứa từ khóa nhạy cảm
            var bannedWords = new[] { "admin", "root", "system", "moderator", "test" };
            if (bannedWords.Any(w => model.TenDN.ToLower().Contains(w)))
            {
                ModelState.AddModelError(nameof(model.TenDN), "Tên đăng nhập chứa từ khóa không được phép!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2.6. Validate họ tên không chứa ký tự đặc biệt
            if (!string.IsNullOrWhiteSpace(model.HoTen) &&
                Regex.IsMatch(model.HoTen, @"[^a-zA-ZÀ-ỹ\s]"))
            {
                ModelState.AddModelError(nameof(model.HoTen), "Họ tên chỉ được chứa chữ cái và khoảng trắng!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2.7. Validate tuổi hợp lệ
            if (model.NgaySinh.HasValue)
            {
                var age = DateTime.Today.Year - model.NgaySinh.Value.Year;
                if (model.NgaySinh.Value > DateTime.Today.AddYears(-age)) age--;

                if (age < 16 || age > 100)
                {
                    ModelState.AddModelError(nameof(model.NgaySinh), "Tuổi phải từ 16-100!");
                    return View("~/Views/Register/Register.cshtml", model);
                }
            }

            // 2.8. Validate mật khẩu mạnh
            if (!IsStrongPassword(model.MatKhau))
            {
                ModelState.AddModelError(nameof(model.MatKhau),
                    "Mật khẩu phải chứa ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // 2.9. Validate email format chính xác
            if (!IsValidEmail(model.Email))
            {
                ModelState.AddModelError(nameof(model.Email), "Email không đúng định dạng!");
                return View("~/Views/Register/Register.cshtml", model);
            }

            // ===== 3. TẠO TÀI KHOẢN =====
            using var transaction = _context.Database.BeginTransaction();
            try
            {
                // 3.1. Tạo ThanhVien
                var thanhVien = new ThanhVien
                {
                    HoTen = model.HoTen,
                    NgaySinh = model.NgaySinh.HasValue ? DateOnly.FromDateTime(model.NgaySinh.Value) : null,
                    GioiTinh = string.IsNullOrWhiteSpace(model.GioiTinh) ? null : model.GioiTinh,
                    Sdt = model.SDT,
                    DiaChi = string.IsNullOrWhiteSpace(model.DiaChi) ? null : model.DiaChi,
                    Khoa = string.IsNullOrWhiteSpace(model.Khoa) ? null : model.Khoa,
                    Lop = string.IsNullOrWhiteSpace(model.Lop) ? null : model.Lop,
                    Email = model.Email,
                    VaiTro = string.IsNullOrWhiteSpace(model.VaiTro) ? "Thành viên" : model.VaiTro,
                    NgayThamGia = DateOnly.FromDateTime(DateTime.Now),
                    TrangThai = "Hoạt động"
                };

                _context.ThanhViens.Add(thanhVien);
                _context.SaveChanges();

                // 3.2. Tạo TaiKhoan
                var taiKhoan = new TaiKhoan
                {
                    TenDn = model.TenDN,
                    MatKhau = PasswordHelper.HashPassword(model.MatKhau),
                    NgayTao = DateTime.Now,
                    TrangThai = "Hoạt động",
                    QuyenHan = DetermineQuyenHan(model.VaiTro),
                    MaTv = thanhVien.MaTv
                };

                _context.TaiKhoans.Add(taiKhoan);
                _context.SaveChanges();
                transaction.Commit();

                TempData["Success"] = "Đăng ký thành công! Vui lòng đăng nhập để tiếp tục.";
                return RedirectToAction("Login", "Auth");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                ModelState.AddModelError("", "Có lỗi xảy ra khi tạo tài khoản. Vui lòng thử lại!");

                // Log error for debugging (uncomment in development)
                // ModelState.AddModelError("", $"Chi tiết lỗi: {ex.Message}");

                return View("~/Views/Register/Register.cshtml", model);
            }
        }

        // ===== HELPER METHODS =====

        /// <summary>
        /// Kiểm tra mật khẩu mạnh
        /// </summary>
        private bool IsStrongPassword(string password)
        {
            if (string.IsNullOrWhiteSpace(password) || password.Length < 8)
                return false;

            bool hasUpper = password.Any(char.IsUpper);
            bool hasLower = password.Any(char.IsLower);
            bool hasDigit = password.Any(char.IsDigit);
            bool hasSpecial = password.Any(ch => !char.IsLetterOrDigit(ch));

            return hasUpper && hasLower && hasDigit && hasSpecial;
        }

        /// <summary>
        /// Kiểm tra email hợp lệ
        /// </summary>
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;

            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Xác định quyền hạn dựa trên vai trò
        /// </summary>
        private string DetermineQuyenHan(string? vaiTro)
        {
            return vaiTro switch
            {
                "Ứng viên" => "Ứng viên",
                "Thành viên" => "Thành viên",
                _ => "Thành viên"
            };
        }
    }
}