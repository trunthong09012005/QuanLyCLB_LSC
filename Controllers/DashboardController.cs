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

            // Activities for selected year - fetch and compute displayed status similarly to ActivitiesController
            var rawActivities = _context.HoatDongs
                .Where(h => h.NgayToChuc.HasValue && h.NgayToChuc.Value.Year == selectedYear)
                .OrderByDescending(h => h.NgayToChuc)
                .ToList();

            var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
            var activitiesForYear = rawActivities.Select(h =>
            {
                string status;
                if (!string.IsNullOrWhiteSpace(h.TrangThai))
                {
                    status = h.TrangThai!;
                }
                else
                {
                    if (h.NgayToChuc.HasValue)
                    {
                        var d = h.NgayToChuc.Value;
                        if (d < todayDateOnly)
                        {
                            status = "Đã tổ chức";
                        }
                        else if (d == todayDateOnly)
                        {
                            status = "Đang diễn ra";
                        }
                        else
                        {
                            var target = d.ToDateTime(System.TimeOnly.MinValue);
                            var hoursUntil = (target - DateTime.Now).TotalHours;
                            if (hoursUntil <= 48)
                            {
                                status = "Sắp diễn ra";
                            }
                            else
                            {
                                status = "Đang chuẩn bị";
                            }
                        }
                    }
                    else
                    {
                        status = "Chưa rõ";
                    }
                }

                return new
                {
                    h.MaHd,
                    h.TenHd,
                    h.NgayToChuc,
                    h.DiaDiem,
                    TrangThai = status
                };
            }).ToList();

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

            // ✅ Top DiemRenLuyen - bảng xếp hạng (lấy 5 cao nhất)
            var topDiemRenLuyen = _context.DiemRenLuyens
                .Include(d => d.MaTvNavigation)
                .Where(d => d.Diem != null)
                .OrderByDescending(d => d.Diem)
                .Take(5)
                .Select(d => new
                {
                    d.MaTv,
                    Ten = d.MaTvNavigation.HoTen,
                    Diem = d.Diem ?? 0,
                    d.HocKy,
                    d.NamHoc
                })
                .ToList();

            // ✅ Thông báo mới (5)
            var thongBaos = _context.ThongBaos
                .OrderByDescending(t => t.NgayDang)
                .Take(5)
                .Select(t => new
                {
                    t.MaTb,
                    t.TieuDe,
                    t.NoiDung,
                    t.NgayDang
                })
                .ToList();

            // ✅ Feedback gần đây (5)
            var recentFeedbacks = _context.Feedbacks
                .Include(f => f.MaTvNavigation)
                .Include(f => f.MaHdNavigation)
                .OrderByDescending(f => f.NgayGopY)
                .Take(5)
                .Select(f => new
                {
                    Ten = f.MaTvNavigation.HoTen,
                    NoiDung = f.NoiDung,
                    Ngay = f.NgayGopY,
                    HoatDong = f.MaHdNavigation != null ? f.MaHdNavigation.TenHd : null
                })
                .ToList();

            // ✅ Lịch sử thao tác gần đây (10)
            var lichSu = _context.LichSuThaoTacs
                .Include(l => l.MaTvNavigation)
                .OrderByDescending(l => l.NgayThucHien)
                .Take(10)
                .Select(l => new
                {
                    l.MaLstt,
                    TenThanhVien = l.MaTvNavigation != null ? l.MaTvNavigation.HoTen : "Hệ thống",
                    l.TenBang,
                    l.LoaiThaoTac,
                    l.KhoaChinh,
                    NoiDung = l.NoiDung,
                    Ngay = l.NgayThucHien
                })
                .ToList();

            // ✅ Khen thưởng gần đây (5)
            var recentKhenThuong = _context.KhenThuongs
                .Include(k => k.MaTvNavigation)
                .OrderByDescending(k => k.NgayKt)
                .Take(5)
                .Select(k => new
                {
                    k.MaKt,
                    MaTv = k.MaTv,
                    Ten = k.MaTvNavigation != null ? k.MaTvNavigation.HoTen : "--",
                    k.LyDo,
                    Ngay = k.NgayKt,
                    NguoiLap = k.NguoiLap != null ? _context.ThanhViens
                                    .Where(tv => tv.MaTv == k.NguoiLap)
                                    .Select(tv => tv.HoTen)
                                    .FirstOrDefault() : null
                })
                .ToList();

            // ✅ Kỷ luật gần đây (5)
            var recentKyLuat = _context.KyLuats
                .Include(k => k.MaTvNavigation)
                .OrderByDescending(k => k.NgayKl)
                .Take(5)
                .Select(k => new
                {
                    k.MaKl,
                    MaTv = k.MaTv,
                    Ten = k.MaTvNavigation != null ? k.MaTvNavigation.HoTen : "--",
                    k.LyDo,
                    Ngay = k.NgayKl,
                    NguoiLap = k.NguoiLap != null ? _context.ThanhViens
                                    .Where(tv => tv.MaTv == k.NguoiLap)
                                    .Select(tv => tv.HoTen)
                                    .FirstOrDefault() : null
                })
                .ToList();

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
            ViewBag.TopDiemRenLuyen = topDiemRenLuyen;
            ViewBag.ThongBaos = thongBaos;
            ViewBag.RecentFeedbacks = recentFeedbacks;
            ViewBag.LichSu = lichSu;
            ViewBag.RecentKhenThuong = recentKhenThuong;
            ViewBag.RecentKyLuat = recentKyLuat;

            return View();
        }
    }
}