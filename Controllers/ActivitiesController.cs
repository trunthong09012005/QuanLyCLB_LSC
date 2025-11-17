using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using System.Text.Json;

namespace QuanLyCLB_LSC.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly QlClbLscContext _context;

        public ActivitiesController(QlClbLscContext context)
        {
            _context = context;
        }

        // GET: Activities
        public async Task<IActionResult> Index(string search, int? loaiId, int? nguoiId)
        {
            var query = _context.HoatDongs
                .Include(h => h.MaLoaiHdNavigation)
                .Include(h => h.NguoiPhuTrachNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(h => h.TenHd.Contains(search) || (h.MoTa != null && h.MoTa.Contains(search)) || (h.DiaDiem != null && h.DiaDiem.Contains(search)));
            }

            if (loaiId.HasValue)
            {
                query = query.Where(h => h.MaLoaiHd == loaiId.Value);
            }

            if (nguoiId.HasValue)
            {
                query = query.Where(h => h.NguoiPhuTrach == nguoiId.Value);
            }

            ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.OrderBy(l => l.TenLoaiHd).ToListAsync();
            ViewBag.ThanhViens = await _context.ThanhViens.OrderBy(t => t.HoTen).ToListAsync();
            ViewBag.Search = search;
            ViewBag.LoaiId = loaiId;
            ViewBag.NguoiId = nguoiId;

            // Stats
            ViewBag.TotalActivities = await _context.HoatDongs.CountAsync();

            var now = DateTime.Now;
            // compute activities this month count
            var dates = (await _context.HoatDongs.Select(h => h.NgayToChuc).ToListAsync()).Where(d => d.HasValue).Select(d => d.Value).ToList();
            ViewBag.ActivitiesThisMonth = dates.Count(d => d.Year == now.Year && d.Month == now.Month);

            ViewBag.ActiveActivities = await _context.HoatDongs.CountAsync(h => h.TrangThai == "?ang chu?n b?");

            // Chart data: counts per month for current year
            var monthlyCounts = new int[12];
            foreach (var d in dates)
            {
                if (d.Year == now.Year)
                {
                    monthlyCounts[d.Month - 1]++;
                }
            }
            var monthLabels = new[] { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };
            ViewBag.ActivityChartLabels = JsonSerializer.Serialize(monthLabels);
            ViewBag.ActivityChartData = JsonSerializer.Serialize(monthlyCounts);

            // Featured / upcoming activities (next 5)
            var allWithDates = await _context.HoatDongs
                .Include(h => h.NguoiPhuTrachNavigation)
                .Where(h => h.NgayToChuc != null)
                .ToListAsync();
            var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
            var upcoming = allWithDates.Where(h => h.NgayToChuc >= todayDateOnly)
                .OrderBy(h => h.NgayToChuc)
                .Take(5)
                .ToList();
            ViewBag.FeaturedActivities = upcoming;

            var list = await query.OrderByDescending(h => h.MaHd).ToListAsync();
            return View(list);
        }

        // GET: Activities/Create
        public IActionResult Create()
        {
            ViewBag.LoaiHoatDongs = _context.LoaiHoatDongs.OrderBy(l => l.TenLoaiHd).ToList();
            ViewBag.ThanhViens = _context.ThanhViens.OrderBy(t => t.HoTen).ToList();
            return View();
        }

        // POST: Activities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TenHd,NgayToChuc,DiaDiem,MoTa,MaLoaiHd,NguoiPhuTrach,KinhPhiDuKien,TrangThai")] HoatDong hoatDong)
        {
            if (ModelState.IsValid)
            {
                _context.Add(hoatDong);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiHoatDongs = _context.LoaiHoatDongs.OrderBy(l => l.TenLoaiHd).ToList();
            ViewBag.ThanhViens = _context.ThanhViens.OrderBy(t => t.HoTen).ToList();
            return View(hoatDong);
        }

        // GET: Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var hd = await _context.HoatDongs
                .Include(h => h.MaLoaiHdNavigation)
                .Include(h => h.NguoiPhuTrachNavigation)
                .FirstOrDefaultAsync(h => h.MaHd == id.Value);
            if (hd == null) return NotFound();
            return View(hd);
        }

        // GET: Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var hd = await _context.HoatDongs.FindAsync(id.Value);
            if (hd == null) return NotFound();
            ViewBag.LoaiHoatDongs = _context.LoaiHoatDongs.OrderBy(l => l.TenLoaiHd).ToList();
            ViewBag.ThanhViens = _context.ThanhViens.OrderBy(t => t.HoTen).ToList();
            return View(hd);
        }

        // POST: Activities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaHd,TenHd,NgayToChuc,DiaDiem,MoTa,MaLoaiHd,NguoiPhuTrach,KinhPhiDuKien,TrangThai")] HoatDong hoatDong)
        {
            if (id != hoatDong.MaHd) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(hoatDong);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.HoatDongs.Any(e => e.MaHd == hoatDong.MaHd)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.LoaiHoatDongs = _context.LoaiHoatDongs.OrderBy(l => l.TenLoaiHd).ToList();
            ViewBag.ThanhViens = _context.ThanhViens.OrderBy(t => t.HoTen).ToList();
            return View(hoatDong);
        }

        // POST: Activities/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var hd = await _context.HoatDongs.FindAsync(id);
            if (hd != null)
            {
                _context.HoatDongs.Remove(hd);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
