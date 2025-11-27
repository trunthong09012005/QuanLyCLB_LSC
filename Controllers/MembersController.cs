using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.Services;
using System.Security.Claims;

namespace QuanLyCLB_LSC.Controllers
{
    public class MembersController : Controller
    {
        private readonly QlClbLscContext _context;
        private readonly IAuditService _audit;

        public MembersController(QlClbLscContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
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

            // normalize sort params and expose to view
            var sortByParam = string.IsNullOrWhiteSpace(sortBy) ? "matv" : sortBy.ToLower();
            // default to descending so most recent items come first
            var sortDirParam = string.IsNullOrWhiteSpace(sortDir) ? "desc" : sortDir.ToLower();
            ViewBag.SortBy = sortByParam;
            ViewBag.SortDir = sortDirParam;

            bool asc = string.Equals(sortDirParam, "asc", StringComparison.OrdinalIgnoreCase);
            query = sortByParam switch
            {
                "hoten" => asc ? query.OrderBy(t => t.HoTen) : query.OrderByDescending(t => t.HoTen),
                "email" => asc ? query.OrderBy(t => t.Email) : query.OrderByDescending(t => t.Email),
                "sdt" => asc ? query.OrderBy(t => t.Sdt) : query.OrderByDescending(t => t.Sdt),
                "ngaythamgia" => asc ? query.OrderBy(t => t.NgayThamGia) : query.OrderByDescending(t => t.NgayThamGia),
                _ => asc ? query.OrderBy(t => t.MaTv) : query.OrderByDescending(t => t.MaTv),
            };

            ViewBag.ChucVus = await _context.ChucVus.OrderBy(c => c.TenCv).ToListAsync();
            ViewBag.Bans = await _context.BanChuyenMons.OrderBy(b => b.TenBan).ToListAsync();
            ViewBag.Search = search;
            ViewBag.CvId = cvId;
            ViewBag.BanId = banId;

            // Statistics for the header cards
            ViewBag.TongThanhVien = await _context.ThanhViens.CountAsync();
            ViewBag.TongTaiKhoan = await _context.TaiKhoans.CountAsync();

            // Count number of ThanhVien whose TrangThai indicates active membership ("Hoạt động")
            var activeStatus = "Hoạt động";
            int thanhVienHoatDong = 0;
            try
            {
                // try equality with trim (may translate depending on provider)
                thanhVienHoatDong = await _context.ThanhViens.CountAsync(tv => tv.TrangThai != null && tv.TrangThai.Trim() == activeStatus);
            }
            catch
            {
                // fallback to SQL LIKE which is translatable
                thanhVienHoatDong = await _context.ThanhViens.CountAsync(tv => tv.TrangThai != null && EF.Functions.Like(tv.TrangThai, "%Hoạt động%"));
            }

            ViewBag.HoatDong = thanhVienHoatDong; // used in the view card

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

                // audit
                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                await _audit.LogAsync(userId, "ThanhVien", "Thêm", $"MaTV={thanhVien.MaTv}", $"Thêm thành viên: {thanhVien.HoTen}");

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

                    // audit
                    int? userId = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                    }
                    await _audit.LogAsync(userId, "ThanhVien", "Cập nhật", $"MaTV={thanhVien.MaTv}", $"Cập nhật thành viên: {thanhVien.HoTen}");
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
                var name = tv.HoTen;
                _context.ThanhViens.Remove(tv);
                await _context.SaveChangesAsync();

                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                await _audit.LogAsync(userId, "ThanhVien", "Xóa", $"MaTV={id}", $"Xóa thành viên: {name}");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
