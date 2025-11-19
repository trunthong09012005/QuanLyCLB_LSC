using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using QuanLyCLB_LSC.Helpers;

namespace QuanLyCLB_LSC.Controllers
{
    public class SettingsController : Controller
    {
        private readonly QlClbLscContext _context;

        public SettingsController(QlClbLscContext context)
        {
            _context = context;
        }

        public IActionResult Index(string section = "dashboard")
        {
            // Lấy thông tin cài đặt hệ thống từ database (từ bảng SystemSettings hoặc session)
            var tongThanhVien = _context.ThanhViens.Count();
            var tongHoatDong = _context.HoatDongs.Count();
            var tongDuAn = _context.DuAns.Count();
            var tongTaiKhoan = _context.TaiKhoans.Count();

            // Danh sách chức vụ
            var danhSachChucVu = _context.ChucVus.ToList();

            // Danh sách ban chuyên môn
            var danhSachBan = _context.BanChuyenMons
                .Include(b => b.TruongBanNavigation)
                .ToList();

            // Danh sách loại hoạt động
            var danhSachLoaiHD = _context.LoaiHoatDongs.ToList();

            // Danh sách thành viên cho dropdown trưởng ban
            var danhSachThanhVien = _context.ThanhViens.ToList();

            // Lấy thông tin hệ thống từ session hoặc database
            var tenClb = HttpContext.Session.GetString("TenCLB") ?? "Life Skills Club";
            var emailClb = HttpContext.Session.GetString("EmailCLB") ?? "contact@lsc.edu.vn";
            var sdtClb = HttpContext.Session.GetString("SDTCLB") ?? "0123456789";
            var diaChiClb = HttpContext.Session.GetString("DiaChiCLB") ?? "Đại học XYZ, Thành phố ABC";

            // Lấy thông tin user đang đăng nhập
            var sessionTenDn = HttpContext.Session.GetString("TenDN");
            var tenDnToUse = !string.IsNullOrEmpty(sessionTenDn) ? sessionTenDn : User.Identity?.Name;
            TaiKhoan taiKhoan = null;
            if (!string.IsNullOrEmpty(tenDnToUse))
            {
                taiKhoan = _context.TaiKhoans
                    .Include(tk => tk.MaTvNavigation)
                    .FirstOrDefault(tk => tk.TenDn == tenDnToUse);
            }

            ViewBag.TongThanhVien = tongThanhVien;
            ViewBag.TongHoatDong = tongHoatDong;
            ViewBag.TongDuAn = tongDuAn;
            ViewBag.TongTaiKhoan = tongTaiKhoan;
            ViewBag.DanhSachChucVu = danhSachChucVu;
            ViewBag.DanhSachBan = danhSachBan;
            ViewBag.DanhSachLoaiHD = danhSachLoaiHD;
            ViewBag.DanhSachThanhVien = danhSachThanhVien;
            ViewBag.TenClb = tenClb;
            ViewBag.EmailClb = emailClb;
            ViewBag.SdtClb = sdtClb;
            ViewBag.DiaChiClb = diaChiClb;
            ViewBag.TaiKhoanHienTai = taiKhoan;
            ViewBag.ThanhVienHienTai = taiKhoan?.MaTvNavigation;
            ViewBag.ActiveSection = section;

            return View();
        }

        // ===== QUẢN LÝ CHỨC VỤ =====
        [HttpPost]
        public IActionResult AddChucVu(string tenCV, string moTa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenCV))
                {
                    TempData["Error"] = "Tên chức vụ không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "chucvu" });
                }

                if (_context.ChucVus.Any(c => c.TenCv == tenCV))
                {
                    TempData["Error"] = "Chức vụ này đã tồn tại!";
                    return RedirectToAction(nameof(Index), new { section = "chucvu" });
                }

                var chucVu = new ChucVu
                {
                    TenCv = tenCV,
                    MoTa = moTa
                };
                _context.ChucVus.Add(chucVu);
                _context.SaveChanges();
                TempData["Success"] = "Thêm chức vụ thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm chức vụ: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "chucvu" });
        }

        [HttpPost]
        public IActionResult UpdateChucVu(int id, string tenCV, string moTa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenCV))
                {
                    TempData["Error"] = "Tên chức vụ không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "chucvu" });
                }

                var chucVu = _context.ChucVus.Find(id);
                if (chucVu != null)
                {
                    if (_context.ChucVus.Any(c => c.TenCv == tenCV && c.MaCv != id))
                    {
                        TempData["Error"] = "Chức vụ này đã tồn tại!";
                        return RedirectToAction(nameof(Index), new { section = "chucvu" });
                    }

                    chucVu.TenCv = tenCV;
                    chucVu.MoTa = moTa;
                    _context.SaveChanges();
                    TempData["Success"] = "Cập nhật chức vụ thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật chức vụ: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "chucvu" });
        }

        [HttpPost]
        public IActionResult DeleteChucVu(int id)
        {
            try
            {
                var chucVu = _context.ChucVus.Find(id);
                if (chucVu != null)
                {
                    var hasMembers = _context.ThanhViens.Any(tv => tv.MaCv == id);
                    if (hasMembers)
                    {
                        TempData["Error"] = "Không thể xóa chức vụ này vì có thành viên đang đảm nhận!";
                        return RedirectToAction(nameof(Index), new { section = "chucvu" });
                    }

                    _context.ChucVus.Remove(chucVu);
                    _context.SaveChanges();
                    TempData["Success"] = "Xóa chức vụ thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa chức vụ: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "chucvu" });
        }

        // ===== QUẢN LÝ BAN CHUYÊN MÔN =====
        [HttpPost]
        public IActionResult AddBan(string tenBan, string moTa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenBan))
                {
                    TempData["Error"] = "Tên ban không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "ban" });
                }

                if (_context.BanChuyenMons.Any(b => b.TenBan == tenBan))
                {
                    TempData["Error"] = "Ban này đã tồn tại!";
                    return RedirectToAction(nameof(Index), new { section = "ban" });
                }

                var ban = new BanChuyenMon
                {
                    TenBan = tenBan,
                    MoTa = moTa
                };
                _context.BanChuyenMons.Add(ban);
                _context.SaveChanges();
                TempData["Success"] = "Thêm ban chuyên môn thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm ban chuyên môn: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "ban" });
        }

        [HttpPost]
        public IActionResult UpdateBan(int id, string tenBan, string moTa, int? truongBan)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenBan))
                {
                    TempData["Error"] = "Tên ban không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "ban" });
                }

                var ban = _context.BanChuyenMons.Find(id);
                if (ban != null)
                {
                    if (_context.BanChuyenMons.Any(b => b.TenBan == tenBan && b.MaBan != id))
                    {
                        TempData["Error"] = "Ban này đã tồn tại!";
                        return RedirectToAction(nameof(Index), new { section = "ban" });
                    }

                    ban.TenBan = tenBan;
                    ban.MoTa = moTa;
                    ban.TruongBan = truongBan;
                    _context.SaveChanges();
                    TempData["Success"] = "Cập nhật ban chuyên môn thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật ban chuyên môn: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "ban" });
        }

        [HttpPost]
        public IActionResult DeleteBan(int id)
        {
            try
            {
                var ban = _context.BanChuyenMons.Find(id);
                if (ban != null)
                {
                    var hasMembers = _context.ThanhViens.Any(tv => tv.MaBan == id);
                    if (hasMembers)
                    {
                        TempData["Error"] = "Không thể xóa ban này vì có thành viên đang sử dụng!";
                        return RedirectToAction(nameof(Index), new { section = "ban" });
                    }

                    _context.BanChuyenMons.Remove(ban);
                    _context.SaveChanges();
                    TempData["Success"] = "Xóa ban chuyên môn thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa ban chuyên môn: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "ban" });
        }

        // ===== QUẢN LÝ LOẠI HOẠT ĐỘNG =====
        [HttpPost]
        public IActionResult AddLoaiHoatDong(string tenLoaiHD, string moTa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenLoaiHD))
                {
                    TempData["Error"] = "Tên loại hoạt động không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
                }

                if (_context.LoaiHoatDongs.Any(l => l.TenLoaiHd == tenLoaiHD))
                {
                    TempData["Error"] = "Loại hoạt động này đã tồn tại!";
                    return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
                }

                var loaiHD = new LoaiHoatDong
                {
                    TenLoaiHd = tenLoaiHD,
                    MoTa = moTa
                };
                _context.LoaiHoatDongs.Add(loaiHD);
                _context.SaveChanges();
                TempData["Success"] = "Thêm loại hoạt động thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thêm loại hoạt động: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
        }

        [HttpPost]
        public IActionResult UpdateLoaiHoatDong(int id, string tenLoaiHD, string moTa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenLoaiHD))
                {
                    TempData["Error"] = "Tên loại hoạt động không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
                }

                var loaiHD = _context.LoaiHoatDongs.Find(id);
                if (loaiHD != null)
                {
                    if (_context.LoaiHoatDongs.Any(l => l.TenLoaiHd == tenLoaiHD && l.MaLoaiHd != id))
                    {
                        TempData["Error"] = "Loại hoạt động này đã tồn tại!";
                        return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
                    }

                    loaiHD.TenLoaiHd = tenLoaiHD;
                    loaiHD.MoTa = moTa;
                    _context.SaveChanges();
                    TempData["Success"] = "Cập nhật loại hoạt động thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật loại hoạt động: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
        }

        [HttpPost]
        public IActionResult DeleteLoaiHoatDong(int id)
        {
            try
            {
                var loaiHD = _context.LoaiHoatDongs.Find(id);
                if (loaiHD != null)
                {
                    var hasActivities = _context.HoatDongs.Any(hd => hd.MaLoaiHd == id);
                    if (hasActivities)
                    {
                        TempData["Error"] = "Không thể xóa loại hoạt động này vì có hoạt động đang sử dụng!";
                        return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
                    }

                    _context.LoaiHoatDongs.Remove(loaiHD);
                    _context.SaveChanges();
                    TempData["Success"] = "Xóa loại hoạt động thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi xóa loại hoạt động: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "loaihoatdong" });
        }

        // ===== QUẢN LÝ THÔNG TIN HỆ THỐNG =====
        [HttpPost]
        public IActionResult UpdateSystemInfo(string tenClb, string email, string sdt, string diaChi)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tenClb))
                {
                    TempData["Error"] = "Tên câu lạc bộ không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "hethong" });
                }

                // Lưu thông tin vào session
                HttpContext.Session.SetString("TenCLB", tenClb);
                HttpContext.Session.SetString("EmailCLB", email ?? "");
                HttpContext.Session.SetString("SDTCLB", sdt ?? "");
                HttpContext.Session.SetString("DiaChiCLB", diaChi ?? "");

                TempData["Success"] = "Cập nhật thông tin hệ thống thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật thông tin hệ thống: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "hethong" });
        }

        // ===== QUẢN LÝ THÔNG TIN CÁ NHÂN =====
        [HttpPost]
        public IActionResult UpdateProfile(int maTv, string hoTen, string email, string sdt, string diaChi, string lop, string khoa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(hoTen))
                {
                    TempData["Error"] = "Họ tên không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "canhan" });
                }

                var thanhVien = _context.ThanhViens.Find(maTv);
                if (thanhVien != null)
                {
                    if (!string.IsNullOrWhiteSpace(email))
                    {
                        var emailExists = _context.ThanhViens
                            .Any(tv => tv.Email == email && tv.MaTv != maTv);
                        if (emailExists)
                        {
                            TempData["Error"] = "Email này đã được sử dụng!";
                            return RedirectToAction(nameof(Index), new { section = "canhan" });
                        }
                    }

                    thanhVien.HoTen = hoTen;
                    thanhVien.Email = email;
                    thanhVien.Sdt = sdt;
                    thanhVien.DiaChi = diaChi;
                    thanhVien.Lop = lop;
                    thanhVien.Khoa = khoa;

                    _context.SaveChanges();
                    TempData["Success"] = "Cập nhật thông tin cá nhân thành công!";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi cập nhật thông tin cá nhân: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "canhan" });
        }

        [HttpPost]
        public IActionResult ChangePassword(string tenDn, string matKhauCu, string matKhauMoi, string xacNhanMatKhau)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(matKhauCu) || string.IsNullOrWhiteSpace(matKhauMoi))
                {
                    TempData["Error"] = "Mật khẩu không được để trống!";
                    return RedirectToAction(nameof(Index), new { section = "canhan" });
                }

                if (matKhauMoi != xacNhanMatKhau)
                {
                    TempData["Error"] = "Mật khẩu xác nhận không khớp!";
                    return RedirectToAction(nameof(Index), new { section = "canhan" });
                }

                if (!PasswordHelper.IsStrongPassword(matKhauMoi))
                {
                    TempData["Error"] = "Mật khẩu phải có ít nhất 8 ký tự, bao gồm chữ hoa, chữ thường, số và ký tự đặc biệt!";
                    return RedirectToAction(nameof(Index), new { section = "canhan" });
                }

                var taiKhoan = _context.TaiKhoans.FirstOrDefault(tk => tk.TenDn == tenDn);
                if (taiKhoan == null)
                {
                    TempData["Error"] = "Tài khoản không tồn tại!";
                    return RedirectToAction(nameof(Index), new { section = "canhan" });
                }

                if (!PasswordHelper.VerifyPassword(matKhauCu, taiKhoan.MatKhau))
                {
                    TempData["Error"] = "Mật khẩu cũ không chính xác!";
                    return RedirectToAction(nameof(Index), new { section = "canhan" });
                }

                taiKhoan.MatKhau = PasswordHelper.HashPassword(matKhauMoi);
                _context.SaveChanges();

                TempData["Success"] = "Thay đổi mật khẩu thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Lỗi khi thay đổi mật khẩu: " + ex.Message;
            }
            return RedirectToAction(nameof(Index), new { section = "canhan" });
        }
    }
}