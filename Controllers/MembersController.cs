using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;

namespace QuanLyCLB_LSC.Controllers
{
    public class MembersController : Controller
    {
        private readonly QlClbLscContext _context;

        public MembersController(QlClbLscContext context)
        {
            _context = context;
        }

        // GET: Members
        public async Task<IActionResult> Index(string search, int? cvId, int? banId, string? sortBy, string? sortDir)
        {
            var query = _context.ThanhViens
                .Include(t => t.MaCvNavigation)
                .Include(t => t.MaBanNavigation)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(t => t.HoTen.Contains(search) || (t.Email != null && t.Email.Contains(search)) || (t.Sdt != null && t.Sdt.Contains(search)));
            }

            if (cvId.HasValue)
            {
                query = query.Where(t => t.MaCv == cvId.Value);
            }

            if (banId.HasValue)
            {
                query = query.Where(t => t.MaBan == banId.Value);
            }

            // expose current sorting to view
            ViewBag.SortBy = sortBy;
            ViewBag.SortDir = sortDir;

            // Apply sorting
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                query = sortBy.ToLower() switch
                {
                    "hoten" => asc ? query.OrderBy(t => t.HoTen) : query.OrderByDescending(t => t.HoTen),
                    "email" => asc ? query.OrderBy(t => t.Email) : query.OrderByDescending(t => t.Email),
                    "sdt" => asc ? query.OrderBy(t => t.Sdt) : query.OrderByDescending(t => t.Sdt),
                    "ngaythamgia" => asc ? query.OrderBy(t => t.NgayThamGia) : query.OrderByDescending(t => t.NgayThamGia),
                    _ => asc ? query.OrderBy(t => t.MaTv) : query.OrderByDescending(t => t.MaTv),
                };
            }
            else
            {
                query = query.OrderByDescending(t => t.MaTv);
            }

            ViewBag.ChucVus = await _context.ChucVus.OrderBy(c => c.TenCv).ToListAsync();
            ViewBag.Bans = await _context.BanChuyenMons.OrderBy(b => b.TenBan).ToListAsync();
            ViewBag.Search = search;
            ViewBag.CvId = cvId;
            ViewBag.BanId = banId;

            // Statistics for the header cards
            ViewBag.TongThanhVien = await _context.ThanhViens.CountAsync();
            ViewBag.TongTaiKhoan = await _context.TaiKhoans.CountAsync();
            // Count active accounts robustly: prefer TaiKhoan.TrangThai, trim whitespace if supported; fallback to counting accounts whose ThanhVien.TrangThai == "Ho?t ??ng"
            long activeAccounts = 0;
            try
            {
                activeAccounts = await _context.TaiKhoans.CountAsync(t => t.TrangThai != null && t.TrangThai.Trim() == "Ho?t ??ng");
            }
            catch
            {
                // If Trim() isn't translatable, use LIKE as fallback
                activeAccounts = await _context.TaiKhoans.CountAsync(t => t.TrangThai != null && EF.Functions.Like(t.TrangThai, "%Ho?t ??ng%"));
            }

            if (activeAccounts == 0)
            {
                activeAccounts = await _context.TaiKhoans
                    .Include(t => t.MaTvNavigation)
                    .CountAsync(t => t.MaTvNavigation != null && t.MaTvNavigation.TrangThai == "Ho?t ??ng");
            }
            ViewBag.HoatDong = activeAccounts;

            var list = await query.ToListAsync();
            return View(list);
        }

        // GET: Members/Create
        public IActionResult Create()
        {
            ViewBag.ChucVus = _context.ChucVus.OrderBy(c => c.TenCv).ToList();
            ViewBag.Bans = _context.BanChuyenMons.OrderBy(b => b.TenBan).ToList();
            return View();
        }

        // POST: Members/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HoTen,NgaySinh,GioiTinh,Lop,Khoa,Sdt,Email,DiaChi,VaiTro,MaCv,MaBan,TrangThai")] ThanhVien thanhVien)
        {
            if (ModelState.IsValid)
            {
                _context.Add(thanhVien);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ChucVus = _context.ChucVus.OrderBy(c => c.TenCv).ToList();
            ViewBag.Bans = _context.BanChuyenMons.OrderBy(b => b.TenBan).ToList();
            return View(thanhVien);
        }

        // GET: Members/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var tv = await _context.ThanhViens
                .Include(t => t.MaCvNavigation)
                .Include(t => t.MaBanNavigation)
                .FirstOrDefaultAsync(m => m.MaTv == id.Value);
            if (tv == null) return NotFound();
            return View(tv);
        }

        // GET: Members/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var tv = await _context.ThanhViens.FindAsync(id.Value);
            if (tv == null) return NotFound();
            ViewBag.ChucVus = _context.ChucVus.OrderBy(c => c.TenCv).ToList();
            ViewBag.Bans = _context.BanChuyenMons.OrderBy(b => b.TenBan).ToList();
            return View(tv);
        }

        // POST: Members/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MaTv,HoTen,NgaySinh,GioiTinh,Lop,Khoa,Sdt,Email,DiaChi,VaiTro,MaCv,MaBan,TrangThai")] ThanhVien thanhVien)
        {
            if (id != thanhVien.MaTv) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(thanhVien);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.ThanhViens.Any(e => e.MaTv == thanhVien.MaTv)) return NotFound();
                    throw;
                }
                return RedirectToAction(nameof(Index));
            }
            ViewBag.ChucVus = _context.ChucVus.OrderBy(c => c.TenCv).ToList();
            ViewBag.Bans = _context.BanChuyenMons.OrderBy(b => b.TenBan).ToList();
            return View(thanhVien);
        }

        // POST: Members/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tv = await _context.ThanhViens.FindAsync(id);
            if (tv != null)
            {
                _context.ThanhViens.Remove(tv);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
