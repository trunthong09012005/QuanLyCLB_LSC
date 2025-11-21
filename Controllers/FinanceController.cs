using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;
using System.Text;
using System.Text.Json;

namespace QuanLyCLB_LSC.Controllers
{
    public class FinanceController : Controller
    {
        private readonly QlClbLscContext _context;

        public FinanceController(QlClbLscContext context)
        {
            _context = context;
        }
        public IActionResult Index()
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
            var giaoDichGanDay = (from tc in _context.ThuChis
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
                     .Take(10)
                     .ToList();

            ViewBag.GiaoDichGanDay = giaoDichGanDay;

            // Lấy tất cả giao dịch cho bảng chi tiết
            var danhSachGiaoDich = (from tc in _context.ThuChis
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
                                    }).ToList();

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
    }
}
