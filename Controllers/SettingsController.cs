using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Controllers
{
    public class SettingsController : Controller
    {
        private readonly QlClbLscContext _context;

        public SettingsController(QlClbLscContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Lấy thông tin cài đặt hệ thống
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

            ViewBag.TongThanhVien = tongThanhVien;
            ViewBag.TongHoatDong = tongHoatDong;
            ViewBag.TongDuAn = tongDuAn;
            ViewBag.TongTaiKhoan = tongTaiKhoan;
            ViewBag.DanhSachChucVu = danhSachChucVu;
            ViewBag.DanhSachBan = danhSachBan;
            ViewBag.DanhSachLoaiHD = danhSachLoaiHD;

            return View();
        }

        // Quản lý chức vụ
        [HttpPost]
        public IActionResult AddChucVu(string tenCV, string moTa)
        {
            var chucVu = new ChucVu
            {
                TenCv = tenCV,
                MoTa = moTa
            };
            _context.ChucVus.Add(chucVu);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Quản lý ban chuyên môn
        [HttpPost]
        public IActionResult AddBan(string tenBan, string moTa)
        {
            var ban = new BanChuyenMon
            {
                TenBan = tenBan,
                MoTa = moTa
            };
            _context.BanChuyenMons.Add(ban);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Quản lý loại hoạt động
        [HttpPost]
        public IActionResult AddLoaiHoatDong(string tenLoaiHD, string moTa)
        {
            var loaiHD = new LoaiHoatDong
            {
                TenLoaiHd = tenLoaiHD,
                MoTa = moTa
            };
            _context.LoaiHoatDongs.Add(loaiHD);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        // Cập nhật thông tin hệ thống
        [HttpPost]
        public IActionResult UpdateSystemInfo(string tenClb, string email, string sdt, string diaChi)
        {
            // Lưu thông tin vào session hoặc database
            TempData["Success"] = "Cập nhật thông tin hệ thống thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}