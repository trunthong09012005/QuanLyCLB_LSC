using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly QlClbLscContext _context;

        public DashboardController(QlClbLscContext context)
        {
            _context = context;
        }

        public IActionResult Index(int? year)
        {
            // ✅ Tổng thành viên
            var tongThanhVien = _context.ThanhViens.Count();

            // determine selected year (default to current)
            var currentYear = DateTime.Now.Year;
            var selectedYear = year ?? currentYear;

            // Available years 2020-2025
            var availableYears = Enumerable.Range(2020, 6).OrderByDescending(y => y).ToList();

            // ✅ Hoạt động tháng này (for current month/year)
            var thangNay = DateTime.Now.Month;
            var namNay = DateTime.Now.Year;
            var hoatDongThangNay = _context.HoatDongs
                .Where(h => h.NgayToChuc.HasValue &&
                           h.NgayToChuc.Value.Month == thangNay &&
                           h.NgayToChuc.Value.Year == namNay)
                .Count();

            // ✅ Dự án đang chạy
            var duAnDangChay = _context.DuAns
                .Where(d => d.TrangThai == "Đang thực hiện")
                .Count();

            // ✅ Tổng ngân sách (tổng thu - tổng chi)
            var tongThu = _context.ThuChis
                .Where(t => t.LoaiGd == "Thu")
                .Sum(t => (decimal?)t.SoTien) ?? 0;

            var tongChi = _context.ThuChis
                .Where(t => t.LoaiGd == "Chi")
                .Sum(t => (decimal?)t.SoTien) ?? 0;

            var nganSach = tongThu - tongChi;

            // ✅ Số thông báo chưa đọc
            var thongBaoMoi = _context.ThongBaos
                .OrderByDescending(t => t.NgayDang)
                .Take(5)
                .Count();

            // ✅ Hoạt động gần đây (5 hoạt động mới nhất)
            var hoatDongGanDay = _context.HoatDongs
                .OrderByDescending(h => h.NgayToChuc)
                .Take(5)
                .Select(h => new
                {
                    h.TenHd,
                    h.NgayToChuc,
                    h.DiaDiem,
                    h.TrangThai,
                    SoNguoiThamGia = _context.ThamGia
                        .Where(t => t.MaHd == h.MaHd)
                        .Count()
                })
                .ToList();

            // Activities for selected year
            var activitiesForYear = _context.HoatDongs
                .Where(h => h.NgayToChuc.HasValue && h.NgayToChuc.Value.Year == selectedYear)
                .OrderByDescending(h => h.NgayToChuc)
                .Select(h => new
                {
                    h.MaHd,
                    h.TenHd,
                    h.NgayToChuc,
                    h.DiaDiem,
                    h.TrangThai
                })
                .ToList();

            // ✅ Dữ liệu biểu đồ - Hoạt động theo tháng for selectedYear
            var hoatDongTheoThang = Enumerable.Range(1, 12)
                .Select(thang => _context.HoatDongs
                    .Where(h => h.NgayToChuc.HasValue &&
                               h.NgayToChuc.Value.Month == thang &&
                               h.NgayToChuc.Value.Year == selectedYear)
                    .Count())
                .ToList();

            // ✅ Dữ liệu biểu đồ - Thành viên theo ban
            var thanhVienTheoBan = _context.BanChuyenMons
                .Select(b => new
                {
                    TenBan = b.TenBan,
                    SoLuong = _context.ThanhViens
                        .Where(tv => tv.MaBan == b.MaBan)
                        .Count()
                })
                .ToList();

            // Member counts by role (group by VaiTro)
            var memberRoleGroups = _context.ThanhViens
                .GroupBy(tv => tv.VaiTro ?? "Chưa phân loại")
                .Select(g => new { Role = g.Key, Count = g.Count() })
                .ToList();
            var memberRoleLabels = memberRoleGroups.Select(m => m.Role).ToList();
            var memberRoleData = memberRoleGroups.Select(m => m.Count).ToList();

            // Gửi dữ liệu sang View
            ViewBag.TongThanhVien = tongThanhVien;
            ViewBag.HoatDongThangNay = hoatDongThangNay;
            ViewBag.DuAnDangChay = duAnDangChay;
            ViewBag.NganSach = nganSach >= 1000000
                ? $"{nganSach / 1000000:F1}M"
                : $"{nganSach / 1000:F0}K";
            ViewBag.ThongBaoMoi = thongBaoMoi;
            ViewBag.HoatDongGanDay = hoatDongGanDay;
            ViewBag.HoatDongTheoThang = hoatDongTheoThang;
            ViewBag.ThanhVienTheoBan = thanhVienTheoBan;
            ViewBag.AvailableYears = availableYears;
            ViewBag.SelectedYear = selectedYear;
            ViewBag.ActivitiesForYear = activitiesForYear;
            ViewBag.MemberRoleLabels = memberRoleLabels;
            ViewBag.MemberRoleData = memberRoleData;

            return View();
        }
    }
}