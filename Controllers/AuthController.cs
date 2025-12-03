using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using Microsoft.AspNetCore.Http;
using QuanLyCLB_LSC.Helpers;
using QuanLyCLB_LSC.Services;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System;
using Microsoft.Extensions.Caching.Memory;

namespace QuanLyCLB_LSC.Controllers
{
    public class AuthController : Controller
    {
        private readonly QlClbLscContext _context;
        private readonly IAuditService _audit;
        private readonly IMemoryCache _cache;
        private const int MAX_LOGIN_ATTEMPTS = 5;
        private const int LOCKOUT_DURATION_MINUTES = 15;
        private const int ATTEMPT_WINDOW_MINUTES = 15;

        public AuthController(QlClbLscContext context, IAuditService audit, IMemoryCache cache)
        {
            _context = context;
            _audit = audit;
            _cache = cache;
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetInt32("MaTV").HasValue)
            {
                var role = HttpContext.Session.GetString("QuyenHan");
                if (role == "Quản trị viên" || role == "Admin")
                    return RedirectToAction("Index", "Dashboard");
                else
                    return RedirectToAction("User", "UserDashboard");
            }

            return View("~/Views/Auth/Login.cshtml");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Error = "Vui lòng kiểm tra lại thông tin đăng nhập!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            model.TenDN = model.TenDN?.Trim();
            if (string.IsNullOrWhiteSpace(model.TenDN) || string.IsNullOrWhiteSpace(model.MatKhau))
            {
                ViewBag.Error = "Tên đăng nhập và mật khẩu không được để trống!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            if (ContainsSqlInjectionPattern(model.TenDN) || ContainsSqlInjectionPattern(model.MatKhau))
            {
                ViewBag.Error = "Phát hiện ký tự không hợp lệ!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            var ipAddress = GetClientIpAddress();

            // ============================================================
            // KIỂM TRA RATE LIMITING VỚI MEMORY CACHE
            // ============================================================
            var lockoutKey = $"Lockout_{model.TenDN}_{ipAddress}";
            var attemptKey = $"Attempts_{model.TenDN}_{ipAddress}";

            // Kiểm tra lockout
            if (_cache.TryGetValue(lockoutKey, out DateTime lockoutEnd))
            {
                if (DateTime.Now < lockoutEnd)
                {
                    var remainingMinutes = (lockoutEnd - DateTime.Now).Minutes + 1;
                    ViewBag.Error = $"Tài khoản tạm thời bị khóa. Vui lòng thử lại sau {remainingMinutes} phút.";
                    return View("~/Views/Auth/Login.cshtml", model);
                }
                else
                {
                    // Lockout đã hết hạn
                    _cache.Remove(lockoutKey);
                    _cache.Remove(attemptKey);
                }
            }

            var user = await _context.TaiKhoans
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.TenDn == model.TenDN);

            if (user == null)
            {
                // Tăng số lần thử
                var attempts = IncrementLoginAttempts(attemptKey);

                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                await _audit.LogAsync(null, "TaiKhoan", "Đăng nhập thất bại",
                    $"TenDN={model.TenDN}, IP={ipAddress}, Attempts={attempts}", "Tên đăng nhập không tồn tại");

                // Kiểm tra và áp dụng lockout
                CheckAndApplyLockout(lockoutKey, attemptKey, attempts);

                return View("~/Views/Auth/Login.cshtml", model);
            }

            var storedHash = user.MatKhau ?? string.Empty;
            var inputHash = PasswordHelper.HashPassword(model.MatKhau);

            if (!string.Equals(inputHash, storedHash, System.StringComparison.OrdinalIgnoreCase))
            {
                // Tăng số lần thử
                var attempts = IncrementLoginAttempts(attemptKey);

                ViewBag.Error = "Tên đăng nhập hoặc mật khẩu không đúng!";
                await _audit.LogAsync(user.MaTv, "TaiKhoan", "Đăng nhập thất bại",
                    $"MaTV={user.MaTv}, IP={ipAddress}, Attempts={attempts}", "Mật khẩu không đúng");

                // Kiểm tra và áp dụng lockout
                CheckAndApplyLockout(lockoutKey, attemptKey, attempts);

                return View("~/Views/Auth/Login.cshtml", model);
            }

            if (string.IsNullOrWhiteSpace(user.QuyenHan) ||
                !(user.QuyenHan == "Quản trị viên" || user.QuyenHan == "Admin" ||
                  user.QuyenHan == "Member" || user.QuyenHan == "Thành viên"))
            {
                ViewBag.Error = "Tài khoản chưa được phân quyền!";
                return View("~/Views/Auth/Login.cshtml", model);
            }

            // ============================================================
            // ĐĂNG NHẬP THÀNH CÔNG - XÓA ATTEMPTS
            // ============================================================
            _cache.Remove(attemptKey);
            _cache.Remove(lockoutKey);

            HttpContext.Session.SetInt32("MaTV", user.MaTv);
            HttpContext.Session.SetString("TenDN", user.TenDn);
            HttpContext.Session.SetString("QuyenHan", user.QuyenHan);
            HttpContext.Session.SetString("LoginTime", DateTime.Now.ToString());

            if (model.RememberMe)
            {
                HttpContext.Response.Cookies.Append("RememberMe", user.MaTv.ToString(),
                    new CookieOptions
                    {
                        Expires = DateTimeOffset.Now.AddDays(30),
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
            }

            await _audit.LogAsync(user.MaTv, "TaiKhoan", "Đăng nhập thành công",
                $"MaTV={user.MaTv}, IP={ipAddress}", $"Đăng nhập: {user.TenDn}");

            var isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";

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
                if (isAjax) return Json(new { success = false, message = "Quyền không hợp lệ!" });
                ViewBag.Error = "Quyền không hợp lệ!";
                return View("~/Views/Auth/Login.cshtml", model);
            }
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            int? maTv = HttpContext.Session.GetInt32("MaTV");
            var ipAddress = GetClientIpAddress();

            if (maTv.HasValue)
            {
                await _audit.LogAsync(maTv, "TaiKhoan", "Đăng xuất",
                    $"MaTV={maTv}, IP={ipAddress}", "Đăng xuất");
            }

            HttpContext.Session.Clear();

            if (HttpContext.Request.Cookies.ContainsKey("RememberMe"))
            {
                HttpContext.Response.Cookies.Delete("RememberMe");
            }

            return RedirectToAction("Login");
        }

        // ============================================================
        // PRIVATE METHODS - MEMORY CACHE RATE LIMITING
        // ============================================================

        /// <summary>
        /// Tăng số lần thử đăng nhập
        /// </summary>
        private int IncrementLoginAttempts(string attemptKey)
        {
            int attempts = 1;

            if (_cache.TryGetValue(attemptKey, out int existingAttempts))
            {
                attempts = existingAttempts + 1;
            }

            // Lưu với thời gian tồn tại = ATTEMPT_WINDOW_MINUTES
            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(ATTEMPT_WINDOW_MINUTES));

            _cache.Set(attemptKey, attempts, cacheOptions);

            return attempts;
        }

        /// <summary>
        /// Kiểm tra và áp dụng lockout
        /// </summary>
        private void CheckAndApplyLockout(string lockoutKey, string attemptKey, int attempts)
        {
            if (attempts >= MAX_LOGIN_ATTEMPTS)
            {
                var lockoutEnd = DateTime.Now.AddMinutes(LOCKOUT_DURATION_MINUTES);

                // Lưu thời gian lockout
                _cache.Set(lockoutKey, lockoutEnd, lockoutEnd - DateTime.Now);

                ViewBag.Error = $"Bạn đã nhập sai mật khẩu quá {MAX_LOGIN_ATTEMPTS} lần. Tài khoản tạm thời bị khóa trong {LOCKOUT_DURATION_MINUTES} phút.";
            }
            else
            {
                var remainingAttempts = MAX_LOGIN_ATTEMPTS - attempts;
                ViewBag.RemainingAttempts = $"Còn {remainingAttempts} lần thử.";
            }
        }

        private string GetClientIpAddress()
        {
            var forwardedFor = Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                return forwardedFor.Split(',')[0].Trim();
            }
            return HttpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        private bool ContainsSqlInjectionPattern(string input)
        {
            if (string.IsNullOrWhiteSpace(input)) return false;

            var sqlPatterns = new[]
            {
                "--", "/*", "*/", "xp_", "sp_", "exec", "execute",
                "drop", "create", "insert", "delete", "update",
                "union", "select", "from", "where", "or 1=1", "or '1'='1",
                "<script", "javascript:", "onerror=", "onload="
            };

            var lowerInput = input.ToLower();
            return sqlPatterns.Any(pattern => lowerInput.Contains(pattern));
        }
    }
}