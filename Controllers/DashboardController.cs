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

            public IActionResult Index()
            {
                // ✅ Tổng thành viên
                var tongThanhVien = _context.ThanhViens.Count();

                // ✅ Hoạt động tháng này
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

                // ✅ Dữ liệu biểu đồ - Hoạt động theo tháng
                var hoatDongTheoThang = Enumerable.Range(1, 12)
                    .Select(thang => _context.HoatDongs
                        .Where(h => h.NgayToChuc.HasValue &&
                                   h.NgayToChuc.Value.Month == thang &&
                                   h.NgayToChuc.Value.Year == namNay)
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

                return View();
            }
        }
    }