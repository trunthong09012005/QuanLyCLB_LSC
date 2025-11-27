using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using System.Text;
using System.Text.Json;
using QuanLyCLB_LSC.Services;
using System.Security.Claims;

namespace QuanLyCLB_LSC.Controllers
{
    public class FinanceController : Controller
    {
        private readonly QlClbLscContext _context;
        private readonly IAuditService _audit;

        public FinanceController(QlClbLscContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }
        public IActionResult Index(string? sortBy, string? sortDir, string? search, string? filterType)
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            // Tính tổng thu tháng này
            var tongThuThang = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Thu" &&
                            tc.NgayGd >= startOfMonth &&
                            tc.NgayGd <= endOfMonth)
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            // Tính tổng chi tháng này
            var tongChiThang = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Chi" &&
                            tc.NgayGd >= startOfMonth &&
                            tc.NgayGd <= endOfMonth)
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            // Tính tổng thu tất cả
            var tongThu = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Thu")
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            // Tính tổng chi tất cả
            var tongChi = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Chi")
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            // Tổng ngân sách = Thu - Chi
            var tongNganSach = tongThu - tongChi;

            // Chênh lệch tháng này
            var chenhLech = tongThuThang - tongChiThang;

            ViewBag.TongNganSach = tongNganSach;
            ViewBag.TongThuThang = tongThuThang;
            ViewBag.TongChiThang = tongChiThang;
            ViewBag.ChenhLech = chenhLech;

            // Lấy giao dịch gần đây (10 giao dịch mới nhất)
            // base query with optional server-side filters
            var baseQuery = _context.ThuChis.AsQueryable();
            if (!string.IsNullOrWhiteSpace(filterType))
            {
                baseQuery = baseQuery.Where(tc => tc.LoaiGd == filterType);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                baseQuery = baseQuery.Where(tc => tc.NoiDung != null && tc.NoiDung.Contains(search));
            }

            var giaoDichGanDayQuery = (from tc in baseQuery
                                  orderby tc.NgayGd descending
                                  select new
                                  {
                                      tc.MaGd,
                                      tc.LoaiGd,
                                      tc.SoTien,
                                      tc.NgayGd,
                                      tc.NoiDung,
                                      NguoiThucHienTen = tc.NguoiThucHien.HasValue ? _context.ThanhViens
                                          .Where(tv => tv.MaTv == tc.NguoiThucHien.Value)
                                          .Select(tv => tv.HoTen)
                                          .FirstOrDefault() : null,
                                      NguonTen = tc.MaNguon.HasValue ? _context.NguonThus
                                          .Where(n => n.MaNguon == tc.MaNguon.Value)
                                          .Select(n => n.TenNguon)
                                          .FirstOrDefault() : null,
                                      HoatDongTen = tc.MaHd.HasValue ? _context.HoatDongs
                                          .Where(hd => hd.MaHd == tc.MaHd.Value)
                                          .Select(hd => hd.TenHd)
                                          .FirstOrDefault() : null
                                  })
                     .AsQueryable();

            // Apply sorting to recent transactions if requested
            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                giaoDichGanDayQuery = sortBy.ToLower() switch
                {
                    "sotien" => asc ? giaoDichGanDayQuery.OrderBy(x => x.SoTien) : giaoDichGanDayQuery.OrderByDescending(x => x.SoTien),
                    "ngaygd" => asc ? giaoDichGanDayQuery.OrderBy(x => x.NgayGd) : giaoDichGanDayQuery.OrderByDescending(x => x.NgayGd),
                    _ => asc ? giaoDichGanDayQuery.OrderBy(x => x.MaGd) : giaoDichGanDayQuery.OrderByDescending(x => x.MaGd),
                };
            }

            var giaoDichGanDay = giaoDichGanDayQuery.Take(10).ToList();

            ViewBag.GiaoDichGanDay = giaoDichGanDay;

            // Lấy tất cả giao dịch cho bảng chi tiết
            var danhSachGiaoDichQuery = (from tc in baseQuery
                                     join tv in _context.ThanhViens on tc.NguoiThucHien equals tv.MaTv into tvJoin
                                     from tv in tvJoin.DefaultIfEmpty()
                                     join n in _context.NguonThus on tc.MaNguon equals n.MaNguon into nJoin
                                     from n in nJoin.DefaultIfEmpty()
                                     join hd in _context.HoatDongs on tc.MaHd equals hd.MaHd into hdJoin
                                     from hd in hdJoin.DefaultIfEmpty()
                                     orderby tc.NgayGd descending
                                     select new
                                     {
                                         tc.MaGd,
                                         tc.LoaiGd,
                                         tc.SoTien,
                                         tc.NgayGd,
                                         tc.NoiDung,
                                         NguoiThucHienTen = tv != null ? tv.HoTen : "N/A",
                                         NguonTen = n != null ? n.TenNguon : null,
                                         HoatDongTen = hd != null ? hd.TenHd : null
                                     }).AsQueryable();

            if (!string.IsNullOrWhiteSpace(sortBy))
            {
                bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                danhSachGiaoDichQuery = sortBy.ToLower() switch
                {
                    "sotien" => asc ? danhSachGiaoDichQuery.OrderBy(x => x.SoTien) : danhSachGiaoDichQuery.OrderByDescending(x => x.SoTien),
                    "ngaygd" => asc ? danhSachGiaoDichQuery.OrderBy(x => x.NgayGd) : danhSachGiaoDichQuery.OrderByDescending(x => x.NgayGd),
                    _ => asc ? danhSachGiaoDichQuery.OrderBy(x => x.MaGd) : danhSachGiaoDichQuery.OrderByDescending(x => x.MaGd),
                };
            }

            var danhSachGiaoDich = danhSachGiaoDichQuery.ToList();

            ViewBag.DanhSachGiaoDich = danhSachGiaoDich;

            // Dữ liệu biểu đồ thu chi theo tháng
            var bieuDoThuChi = new
            {
                thu = Enumerable.Range(1, 12).Select(month =>
                {
                    var startDate = new DateTime(now.Year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    return _context.ThuChis
                        .Where(tc => tc.LoaiGd == "Thu" &&
                                    tc.NgayGd >= startDate &&
                                    tc.NgayGd <= endDate)
                        .Sum(tc => (decimal?)tc.SoTien) ?? 0;
                }).ToArray(),

                chi = Enumerable.Range(1, 12).Select(month =>
                {
                    var startDate = new DateTime(now.Year, month, 1);
                    var endDate = startDate.AddMonths(1).AddDays(-1);

                    return _context.ThuChis
                        .Where(tc => tc.LoaiGd == "Chi" &&
                                    tc.NgayGd >= startDate &&
                                    tc.NgayGd <= endDate)
                        .Sum(tc => (decimal?)tc.SoTien) ?? 0;
                }).ToArray()
            };

            ViewBag.BieuDoThuChiJson = JsonSerializer.Serialize(bieuDoThuChi);

            // Dữ liệu biểu đồ nguồn thu
            var nguonThuStats = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Thu" && tc.MaNguon != null)
                .GroupBy(tc => tc.MaNguon)
                .Select(g => new
                {
                    MaNguon = g.Key,
                    TenNguon = _context.NguonThus
                        .Where(n => n.MaNguon == g.Key)
                        .Select(n => n.TenNguon)
                        .FirstOrDefault(),
                    TongTien = g.Sum(tc => tc.SoTien)
                })
                .ToList();

            var bieuDoNguonThu = new
            {
                labels = nguonThuStats.Select(n => n.TenNguon).ToArray(),
                values = nguonThuStats.Select(n => n.TongTien).ToArray()
            };

            ViewBag.BieuDoNguonThuJson = JsonSerializer.Serialize(bieuDoNguonThu);

            var chiTheoHoatDong = _context.ThuChis
        .Where(tc => tc.LoaiGd == "Chi" && tc.MaHd != null)
        .GroupBy(tc => tc.MaHd)
        .Select(g => new
        {
            MaHd = g.Key,
            TenHoatDong = _context.HoatDongs
                .Where(hd => hd.MaHd == g.Key)
                .Select(hd => hd.TenHd)
                .FirstOrDefault(),
            TongChi = g.Sum(tc => tc.SoTien)
        })
        .Where(x => x.TenHoatDong != null)
        .OrderByDescending(x => x.TongChi)
        .ToList();

            var bieuDoNguonChi = new
            {
                labels = chiTheoHoatDong.Select(x => x.TenHoatDong).ToArray(),
                values = chiTheoHoatDong.Select(x => x.TongChi).ToArray()
            };

            ViewBag.BieuDoNguonChiJson = JsonSerializer.Serialize(bieuDoNguonChi);
            return View();
        }

        // GET: Finance/Details/5
        public IActionResult Details(int id)
        {
            var giaoDich = _context.ThuChis
                .Where(tc => tc.MaGd == id)
                .Select(tc => new
                {
                    tc.MaGd,
                    tc.LoaiGd,
                    tc.SoTien,
                    tc.NgayGd,
                    tc.NoiDung,
                    NguoiThucHienTen = _context.ThanhViens
                        .Where(tv => tv.MaTv == tc.NguoiThucHien)
                        .Select(tv => tv.HoTen)
                        .FirstOrDefault(),
                    NguonTen = _context.NguonThus
                        .Where(n => n.MaNguon == tc.MaNguon)
                        .Select(n => n.TenNguon)
                        .FirstOrDefault(),
                    HoatDongTen = _context.HoatDongs
                        .Where(hd => hd.MaHd == tc.MaHd)
                        .Select(hd => hd.TenHd)
                        .FirstOrDefault(),
                    ChiTiet = _context.ThuChiChiTiets
                        .Where(ct => ct.MaGd == id)
                        .ToList()
                })
                .FirstOrDefault();

            if (giaoDich == null)
            {
                return NotFound();
            }

            return View(giaoDich);
        }

        // GET: Finance/DetailsPartial/5
        public IActionResult DetailsPartial(int id)
        {
            var giaoDich = _context.ThuChis
                .Where(tc => tc.MaGd == id)
                .Select(tc => new
                {
                    tc.MaGd,
                    tc.LoaiGd,
                    tc.SoTien,
                    tc.NgayGd,
                    tc.NoiDung,
                    NguoiThucHienTen = _context.ThanhViens
                        .Where(tv => tv.MaTv == tc.NguoiThucHien)
                        .Select(tv => tv.HoTen)
                        .FirstOrDefault(),
                    NguonTen = _context.NguonThus
                        .Where(n => n.MaNguon == tc.MaNguon)
                        .Select(n => n.TenNguon)
                        .FirstOrDefault(),
                    HoatDongTen = _context.HoatDongs
                        .Where(hd => hd.MaHd == tc.MaHd)
                        .Select(hd => hd.TenHd)
                        .FirstOrDefault(),
                    ChiTiet = _context.ThuChiChiTiets
                        .Where(ct => ct.MaGd == id)
                        .ToList()
                })
                .FirstOrDefault();

            if (giaoDich == null) return NotFound();

            // return a partial view string
            return PartialView("Details", giaoDich);
        }

        // GET: Finance/CreateIncome
        public IActionResult CreateIncome()
        {
            ViewBag.NguonThus = _context.NguonThus.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View();
        }

        // POST: Finance/CreateIncome
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateIncome(ThuChi thuChi)
        {
            if (ModelState.IsValid)
            {
                thuChi.LoaiGd = "Thu";
                thuChi.NgayGd = DateTime.Now;

                _context.ThuChis.Add(thuChi);
                _context.SaveChanges();

                // audit
                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                _audit.LogAsync(userId, "ThuChi", "Thêm", $"MaGD={thuChi.MaGd}", $"Thêm giao dịch Thu: {thuChi.NoiDung} - {thuChi.SoTien}").ConfigureAwait(false);

                TempData["Success"] = "Thêm giao dịch thu thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NguonThus = _context.NguonThus.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View(thuChi);
        }

        // GET: Finance/CreateExpense
        public IActionResult CreateExpense()
        {
            ViewBag.HoatDongs = _context.HoatDongs.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View();
        }

        // POST: Finance/CreateExpense
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateExpense(ThuChi thuChi)
        {
            if (ModelState.IsValid)
            {
                thuChi.LoaiGd = "Chi";
                thuChi.NgayGd = DateTime.Now;

                _context.ThuChis.Add(thuChi);
                _context.SaveChanges();

                // audit
                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                _audit.LogAsync(userId, "ThuChi", "Thêm", $"MaGD={thuChi.MaGd}", $"Thêm giao dịch Chi: {thuChi.NoiDung} - {thuChi.SoTien}").ConfigureAwait(false);

                TempData["Success"] = "Thêm giao dịch chi thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.HoatDongs = _context.HoatDongs.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View(thuChi);
        }

        // GET: Finance/Edit/5
        public IActionResult Edit(int id)
        {
            var thuChi = _context.ThuChis.Find(id);

            if (thuChi == null)
            {
                return NotFound();
            }

            ViewBag.NguonThus = _context.NguonThus.ToList();
            ViewBag.HoatDongs = _context.HoatDongs.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View(thuChi);
        }

        // POST: Finance/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, ThuChi thuChi)
        {
            if (id != thuChi.MaGd)
                return NotFound();

            if (ModelState.IsValid)
            {
                var existing = _context.ThuChis.Find(id);
                if (existing == null)
                    return NotFound();

                // Update các trường cho phép sửa
                existing.SoTien = thuChi.SoTien;
                existing.NoiDung = thuChi.NoiDung;
                existing.NguoiThucHien = thuChi.NguoiThucHien;

                // Thu => chỉnh Nguồn thu
                if (existing.LoaiGd == "Thu")
                {
                    existing.MaNguon = thuChi.MaNguon;
                    existing.MaHd = null; // đảm bảo không nhiễu
                }

                // Chi => chỉnh Hoạt động
                if (existing.LoaiGd == "Chi")
                {
                    existing.MaHd = thuChi.MaHd;
                    existing.MaNguon = null; // đảm bảo không nhiễu
                }

                _context.SaveChanges();

                // audit
                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                _audit.LogAsync(userId, "ThuChi", "Cập nhật", $"MaGD={id}", $"Cập nhật giao dịch: {existing.NoiDung} - {existing.SoTien}").ConfigureAwait(false);

                TempData["Success"] = "Cập nhật giao dịch thành công!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.NguonThus = _context.NguonThus.ToList();
            ViewBag.HoatDongs = _context.HoatDongs.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View(thuChi);
        }


        // GET: Finance/Delete/5
        public IActionResult Delete(int id)
        {
            var thuChi = _context.ThuChis.Find(id);

            if (thuChi == null)
            {
                return NotFound();
            }

            try
            {
                _context.ThuChis.Remove(thuChi);
                _context.SaveChanges();

                // audit
                int? userId = null;
                if (User?.Identity?.IsAuthenticated == true)
                {
                    var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                }
                _audit.LogAsync(userId, "ThuChi", "Xóa", $"MaGD={id}", $"Xóa giao dịch: {thuChi.NoiDung} - {thuChi.SoTien}").ConfigureAwait(false);

                TempData["Success"] = "Xóa giao dịch thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa giao dịch: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Finance/Report
        public IActionResult Report()
        {
            var now = DateTime.Now;
            var startOfMonth = new DateTime(now.Year, now.Month, 1);
            var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);

            var tongThuThang = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Thu" &&
                            tc.NgayGd >= startOfMonth &&
                            tc.NgayGd <= endOfMonth)
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            var tongChiThang = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Chi" &&
                            tc.NgayGd >= startOfMonth &&
                            tc.NgayGd <= endOfMonth)
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            var tongThu = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Thu")
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            var tongChi = _context.ThuChis
                .Where(tc => tc.LoaiGd == "Chi")
                .Sum(tc => (decimal?)tc.SoTien) ?? 0;

            var tongNganSach = tongThu - tongChi;
            var chenhLech = tongThuThang - tongChiThang;

            ViewBag.TongNganSach = tongNganSach;
            ViewBag.TongThuThang = tongThuThang;
            ViewBag.TongChiThang = tongChiThang;
            ViewBag.ChenhLech = chenhLech;

            // Lấy danh sách tất cả giao dịch
            var danhSachGiaoDich = _context.ThuChis
                .OrderByDescending(tc => tc.NgayGd)
                .Select(tc => new
                {
                    tc.MaGd,
                    tc.LoaiGd,
                    tc.SoTien,
                    tc.NgayGd,
                    tc.NoiDung,
                    NguoiThucHienTen = _context.ThanhViens
                        .Where(tv => tv.MaTv == tc.NguoiThucHien)
                        .Select(tv => tv.HoTen)
                        .FirstOrDefault(),
                    NguonTen = _context.NguonThus
                        .Where(n => n.MaNguon == tc.MaNguon)
                        .Select(n => n.TenNguon)
                        .FirstOrDefault(),
                    HoatDongTen = tc.MaHd.HasValue
    ? _context.HoatDongs
        .Where(hd => hd.MaHd == tc.MaHd.Value)
        .Select(hd => hd.TenHd)
        .FirstOrDefault()
    : "(Không có hoạt động)"


                })
                .ToList();

            ViewBag.DanhSachGiaoDich = danhSachGiaoDich;

            return View();
        }

        // GET: Finance/ExportExcel
        public IActionResult ExportExcel()
        {
            var list = _context.ThuChis
                .OrderByDescending(x => x.NgayGd)
                .Select(x => new
                {
                    NoiDung = x.NoiDung ?? "",
                    SoTien = x.SoTien,
                    Ngay = x.NgayGd
                })
                .ToList();

            // Độ rộng cột
            int col1 = 50; // Nội dung
            int col2 = 25; // Số tiền
            int col3 = 15; // Ngày

            var sb = new StringBuilder();

            // Header
            sb.AppendLine(
                "Nội dung".PadRight(col1) +
                "Số tiền".PadLeft(col2) +
                "Ngày".PadLeft(col3)
            );

            sb.AppendLine(new string('-', col1 + col2 + col3));

            // Rows
            foreach (var item in list)
            {
                sb.AppendLine(
                    item.NoiDung.PadRight(col1) +
                    item.SoTien.ToString("N0").PadLeft(col2) +
                    (item.Ngay?.ToString("dd/MM/yyyy") ?? "").PadLeft(col3)
                );
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());
            return File(bytes, "text/plain", "BaoCaoThuChi.txt");
        }

        // GET: Finance/ExportTransaction/5
        public IActionResult ExportTransaction(int id)
        {
            var gd = _context.ThuChis.Find(id);
            if (gd == null) return NotFound();

            var chiTiets = _context.ThuChiChiTiets.Where(ct => ct.MaGd == id).ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Sao kê giao dịch: #GD" + gd.MaGd.ToString("D3"));
            sb.AppendLine("Loại: " + gd.LoaiGd);
            sb.AppendLine("Nội dung: " + (gd.NoiDung ?? ""));
            sb.AppendLine("Số tiền: " + gd.SoTien.ToString("N0") + " VNĐ");
            sb.AppendLine("Ngày: " + (gd.NgayGd.HasValue ? gd.NgayGd.Value.ToString("dd/MM/yyyy HH:mm") : ""));
            sb.AppendLine("Người thực hiện: " + (_context.ThanhViens.Where(t => t.MaTv == gd.NguoiThucHien).Select(t => t.HoTen).FirstOrDefault() ?? "N/A"));
            if (gd.LoaiGd == "Thu")
            {
                sb.AppendLine("Nguồn thu: " + (_context.NguonThus.Where(n => n.MaNguon == gd.MaNguon).Select(n => n.TenNguon).FirstOrDefault() ?? "N/A"));
            }
            if (gd.LoaiGd == "Chi")
            {
                sb.AppendLine("Hoạt động: " + (_context.HoatDongs.Where(h => h.MaHd == gd.MaHd).Select(h => h.TenHd).FirstOrDefault() ?? "N/A"));
            }

            sb.AppendLine();
            sb.AppendLine("Chi tiết giao dịch:");
            sb.AppendLine("--------------------");
            if (!chiTiets.Any()) sb.AppendLine("Không có chi tiết.");
            else
            {
                foreach (var ct in chiTiets)
                {
                    sb.AppendLine($"- [{ct.MaCt}] {ct.NoiDung} : {ct.SoTien.ToString("N0")} VNĐ");
                }
            }

            var bytes = Encoding.UTF8.GetBytes(sb.ToString());

            // audit export
            int? userIdExport = null;
            if (User?.Identity?.IsAuthenticated == true)
            {
                var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (int.TryParse(idClaim, out var parsed)) userIdExport = parsed;
            }
            _audit.LogAsync(userIdExport, "ThuChi", "Xuất khẩu", $"MaGD={id}", $"Xuất sao kê giao dịch: MaGD={id}").ConfigureAwait(false);

            return File(bytes, "text/plain", $"SaoKe_GD{gd.MaGd.ToString("D3")}.txt");
        }
    }
}
