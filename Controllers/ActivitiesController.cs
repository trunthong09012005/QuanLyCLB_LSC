using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using System.Text.Json;
using QuanLyCLB_LSC.Services;
using System.Security.Claims;

namespace QuanLyCLB_LSC.Controllers
{
    public class ActivitiesController : Controller
    {
        private readonly QlClbLscContext _context;
        private readonly IAuditService _audit;

        public ActivitiesController(QlClbLscContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        // GET: Activities
        public async Task<IActionResult> Index(string search, int? loaiId, int? nguoiId, int? year, string? sortBy, string? sortDir)
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

            // expose sorting
            ViewBag.SortBy = sortBy;
            ViewBag.SortDir = sortDir;

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                query = sortBy.ToLower() switch
                {
                    "tenhd" => asc ? query.OrderBy(h => h.TenHd) : query.OrderByDescending(h => h.TenHd),
                    "ngaytochuc" => asc ? query.OrderBy(h => h.NgayToChuc) : query.OrderByDescending(h => h.NgayToChuc),
                    "diadiem" => asc ? query.OrderBy(h => h.DiaDiem) : query.OrderByDescending(h => h.DiaDiem),
                    "nguoi" => asc ? query.OrderBy(h => h.NguoiPhuTrach) : query.OrderByDescending(h => h.NguoiPhuTrach),
                    _ => asc ? query.OrderBy(h => h.MaHd) : query.OrderByDescending(h => h.MaHd),
                };
            }
            else
            {
                query = query.OrderByDescending(h => h.MaHd);
            }

            ViewBag.LoaiHoatDongs = await _context.LoaiHoatDongs.OrderBy(l => l.TenLoaiHd).ToListAsync();
            ViewBag.ThanhViens = await _context.ThanhViens.OrderBy(t => t.HoTen).ToListAsync();
            ViewBag.Search = search;
            ViewBag.LoaiId = loaiId;
            ViewBag.NguoiId = nguoiId;

            // Stats
            ViewBag.TotalActivities = await _context.HoatDongs.CountAsync();

            var now = DateTime.Now;
            // collect all dates (nullable) from DB
            var rawDates = await _context.HoatDongs.Select(h => h.NgayToChuc).ToListAsync();
            var dates = rawDates.Where(d => d.HasValue).Select(d => d.Value).ToList();

            // Available years (from data) + current year if missing
            var availableYears = dates.Select(d => d.Year).Distinct().OrderByDescending(y => y).ToList();
            if (!availableYears.Contains(now.Year))
            {
                availableYears = (new[] { now.Year }).Concat(availableYears).ToList();
            }
            ViewBag.AvailableYears = availableYears;
            var selectedYear = year ?? now.Year;
            ViewBag.SelectedYear = selectedYear;

            // compute activities this month count for current month/year
            ViewBag.ActivitiesThisMonth = dates.Count(d => d.Year == now.Year && d.Month == now.Month);

            // compute statuses based on NgayToChuc relative to today
            var list = await query.ToListAsync();
            var statusMap = new Dictionary<int, string>();
            var todayDateOnly = DateOnly.FromDateTime(DateTime.Today);
            foreach (var h in list)
            {
                string s = "Chưa rõ";
                if (h.NgayToChuc.HasValue)
                {
                    var d = h.NgayToChuc.Value;
                    if (d < todayDateOnly)
                    {
                        s = "Đã tổ chức";
                    }
                    else if (d == todayDateOnly)
                    {
                        s = "Đang diễn ra";
                    }
                    else
                    {
                        // future date
                        // calculate hours until that date (from now)
                        var target = d.ToDateTime(System.TimeOnly.MinValue);
                        var hoursUntil = (target - DateTime.Now).TotalHours;
                        if (hoursUntil <= 48)
                        {
                            s = "Sắp diễn ra";
                        }
                        else
                        {
                            s = "Đang chuẩn bị";
                        }
                    }
                }
                statusMap[h.MaHd] = s;
            }

            ViewBag.ActivityStatuses = statusMap; // Dictionary<int,string>

            // Active/Preparing count
            ViewBag.ActiveActivities = statusMap.Values.Count(v => v == "Đang chuẩn bị");

            // Chart data: counts per month for selected year
            var monthlyCounts = new int[12];
            foreach (var d in dates)
            {
                if (d.Year == selectedYear)
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
            var upcoming = allWithDates.Where(h => h.NgayToChuc >= todayDateOnly)
                .OrderBy(h => h.NgayToChuc)
                .Take(5)
                .ToList();
            ViewBag.FeaturedActivities = upcoming;

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
            // Server-side: disallow creating activities with NgayToChuc earlier than today
            var today = DateOnly.FromDateTime(DateTime.Today);
            if (hoatDong.NgayToChuc.HasValue && hoatDong.NgayToChuc.Value < today)
            {
                ModelState.AddModelError("NgayToChuc", "Ngày tổ chức không thể nhỏ hơn hôm nay.");
            }

            if (ModelState.IsValid)
            {
                _context.Add(hoatDong);
                await _context.SaveChangesAsync();

                // audit log
                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                await _audit.LogAsync(userId, "HoatDong", "Thêm", $"MaHD={hoatDong.MaHd}", $"Tạo hoạt động: {hoatDong.TenHd}");

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

                    int? userId = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                    }
                    await _audit.LogAsync(userId, "HoatDong", "Cập nhật", $"MaHD={hoatDong.MaHd}", $"Cập nhật hoạt động: {hoatDong.TenHd}");
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

                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                await _audit.LogAsync(userId, "HoatDong", "Xóa", $"MaHD={id}", $"Xóa hoạt động: {hd.TenHd}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
