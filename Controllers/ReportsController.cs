using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly QlClbLscContext _context;

        public ReportsController(QlClbLscContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            // Thống kê báo cáo
            var tongBaoCao = _context.BaoCaos.Count();
            var baoCaoThangNay = _context.BaoCaos
                .Where(b => b.NgayLap.HasValue &&
                           b.NgayLap.Value.Month == DateTime.Now.Month &&
                           b.NgayLap.Value.Year == DateTime.Now.Year)
                .Count();

            // Báo cáo gần đây
            var baoCaoGanDay = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .OrderByDescending(b => b.NgayLap)
                .Take(10)
                .Select(b => new
                {
                    b.MaBc,
                    b.TieuDe,
                    b.LoaiBc,
                    b.NgayLap,
                    NguoiLap = b.NguoiLapNavigation != null ? b.NguoiLapNavigation.HoTen : "N/A"
                })
                .ToList();

            // Thống kê theo loại báo cáo
            var thongKeTheoLoai = _context.BaoCaos
                .GroupBy(b => b.LoaiBc)
                .Select(g => new
                {
                    LoaiBaoCao = g.Key ?? "Chưa phân loại",
                    SoLuong = g.Count()
                })
                .ToList();

            ViewBag.TongBaoCao = tongBaoCao;
            ViewBag.BaoCaoThangNay = baoCaoThangNay;
            ViewBag.BaoCaoGanDay = baoCaoGanDay;
            ViewBag.ThongKeTheoLoai = thongKeTheoLoai;

            return View();
        }

        // Xem chi tiết báo cáo
        public IActionResult Details(int id)
        {
            var baoCao = _context.BaoCaos
                .Include(b => b.NguoiLapNavigation)
                .FirstOrDefault(b => b.MaBc == id);

            if (baoCao == null)
            {
                return NotFound();
            }

            return View(baoCao);
        }

        // Tạo báo cáo mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(BaoCao baoCao)
        {
            if (ModelState.IsValid)
            {
                baoCao.NgayLap = DateTime.Now;
                _context.BaoCaos.Add(baoCao);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(baoCao);
        }

        // Xuất báo cáo
        public IActionResult Export(string loai, DateTime? tuNgay, DateTime? denNgay, string dinhDang)
        {
            // Logic xuất báo cáo (PDF, Excel, Word)
            // Tạm thời trả về thông báo
            TempData["Message"] = $"Đang xuất báo cáo {loai} từ {tuNgay} đến {denNgay} dạng {dinhDang}";
            return RedirectToAction(nameof(Index));
        }

        // Xóa báo cáo
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var baoCao = _context.BaoCaos.Find(id);
            if (baoCao != null)
            {
                _context.BaoCaos.Remove(baoCao);
                _context.SaveChanges();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}