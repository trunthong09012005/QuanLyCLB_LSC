using Microsoft.AspNetCore.Mvc;
using QuanLyCLB_LSC.Models;

namespace QuanLyCLB_LSC.Controllers
{
    public class ProjectController : Controller
    {
        private readonly QlClbLscContext _context;

        public ProjectController(QlClbLscContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            try
            {
                // Lấy tổng số dự án
                ViewBag.TongDuAn = _context.DuAns.Count();

                // Đếm dự án đang thực hiện
                ViewBag.DangThucHien = _context.DuAns
                    .Count(da => da.TrangThai == "Đang thực hiện");

                // Đếm dự án sắp đến hạn (còn < 30 ngày)
                var now = DateTime.Now.Date; // Chỉ lấy phần Date để so sánh

                ViewBag.SapDenHan = _context.DuAns
                    .Where(da => da.NgayKetThuc != null &&
                                da.TrangThai == "Đang thực hiện")
                    .AsEnumerable() // Chuyển về client-side để tính toán
                    .Count(da =>
                    {
                        if (!da.NgayKetThuc.HasValue) return false;

                        var daysRemaining = (da.NgayKetThuc.Value.Date - now).Days;
                        return daysRemaining >= 0 && daysRemaining <= 30;
                    });

                // Đếm dự án hoàn thành
                ViewBag.HoanThanh = _context.DuAns
                    .Count(da => da.TrangThai == "Hoàn thành");

                // Lấy danh sách dự án với số thành viên tham gia
                var danhSachDuAn = _context.DuAns
                    .Select(da => new
                    {
                        da.MaDa,
                        da.TenDuAn,
                        da.MoTa,
                        da.NgayBatDau,
                        da.NgayKetThuc,
                        da.TrangThai,
                        SoThanhVien = _context.PhanCongs
                            .Count(pc => pc.MaDa == da.MaDa)
                    })
                    .OrderByDescending(da => da.NgayBatDau)
                    .ToList();

                ViewBag.DanhSachDuAn = danhSachDuAn;

                return View();
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                ViewBag.Error = ex.Message;
                ViewBag.TongDuAn = 0;
                ViewBag.DangThucHien = 0;
                ViewBag.SapDenHan = 0;
                ViewBag.HoanThanh = 0;
                ViewBag.DanhSachDuAn = new List<dynamic>();

                return View();
            }
        }

        // GET: Project/Details/5
        public IActionResult Details(int id)
        {
            try
            {
                var duAn = _context.DuAns
                    .Where(da => da.MaDa == id)
                    .Select(da => new
                    {
                        da.MaDa,
                        da.TenDuAn,
                        da.MoTa,
                        da.NgayBatDau,
                        da.NgayKetThuc,
                        da.TrangThai,
                        ThanhVien = _context.PhanCongs
                            .Where(pc => pc.MaDa == id)
                            .Select(pc => new
                            {
                                pc.MaTv,
                                pc.NhiemVu,
                                pc.TrangThai,
                                HoTen = _context.ThanhViens
                                    .Where(tv => tv.MaTv == pc.MaTv)
                                    .Select(tv => tv.HoTen)
                                    .FirstOrDefault()
                            })
                            .ToList()
                    })
                    .FirstOrDefault();

                if (duAn == null)
                {
                    return NotFound();
                }

                return View(duAn);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return View();
            }
        }

        // GET: Project/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Project/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DuAn duAn)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    _context.DuAns.Add(duAn);
                    _context.SaveChanges();

                    TempData["Success"] = "Tạo dự án thành công!";
                    return RedirectToAction(nameof(Index));
                }

                return View(duAn);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(duAn);
            }
        }

        // GET: Project/Edit/5
        public IActionResult Edit(int id)
        {
            try
            {
                var duAn = _context.DuAns.Find(id);

                if (duAn == null)
                {
                    return NotFound();
                }

                return View(duAn);
            }
            catch (Exception ex)
            {
                ViewBag.Error = ex.Message;
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Project/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DuAn duAn)
        {
            if (id != duAn.MaDa)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    _context.Update(duAn);
                    _context.SaveChanges();

                    TempData["Success"] = "Cập nhật dự án thành công!";
                    return RedirectToAction(nameof(Index));
                }

                return View(duAn);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Có lỗi xảy ra: " + ex.Message);
                return View(duAn);
            }
        }

        // GET: Project/Delete/5
        public IActionResult Delete(int id)
        {
            try
            {
                var duAn = _context.DuAns.Find(id);

                if (duAn == null)
                {
                    return NotFound();
                }

                _context.DuAns.Remove(duAn);
                _context.SaveChanges();

                TempData["Success"] = "Xóa dự án thành công!";
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Không thể xóa dự án: " + ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Project/Assign
        public IActionResult Assign()
        {
            ViewBag.DuAns = _context.DuAns.ToList();
            ViewBag.ThanhViens = _context.ThanhViens.ToList();

            return View();
        }

        // POST: Project/Assign
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Assign(PhanCong phanCong)
        {
            try
            {
                // Bỏ qua validation cho navigation properties
                ModelState.Remove("MaDaNavigation");
                ModelState.Remove("MaTvNavigation");

                // Kiểm tra trùng lặp
                var exists = _context.PhanCongs
                    .Any(pc => pc.MaTv == phanCong.MaTv && pc.MaDa == phanCong.MaDa);

                if (exists)
                {
                    ModelState.AddModelError("", "Thành viên này đã được phân công vào dự án này!");
                    ViewBag.DuAns = _context.DuAns.ToList();
                    ViewBag.ThanhViens = _context.ThanhViens.ToList();
                    return View(phanCong);
                }

                if (ModelState.IsValid)
                {
                    phanCong.NgayPhanCong = DateTime.Now;
                    phanCong.TrangThai = "Chưa hoàn thành";

                    _context.PhanCongs.Add(phanCong);
                    _context.SaveChanges();

                    TempData["Success"] = "Phân công nhiệm vụ thành công!";
                    return RedirectToAction(nameof(Index));
                }

                // Hiển thị lỗi nếu ModelState không valid
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage);
                ViewBag.Error = "Lỗi: " + string.Join(", ", errors);

                ViewBag.DuAns = _context.DuAns.ToList();
                ViewBag.ThanhViens = _context.ThanhViens.ToList();
                return View(phanCong);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Có lỗi xảy ra: " + ex.Message
                    + (ex.InnerException != null ? " - " + ex.InnerException.Message : "");
                ViewBag.DuAns = _context.DuAns.ToList();
                ViewBag.ThanhViens = _context.ThanhViens.ToList();
                return View(phanCong);
            }
        }

        // GET: Project/Export
        public IActionResult Export()
        {
            var duAns = _context.DuAns
        .Select(da => new
        {
            da.MaDa,
            da.TenDuAn,
            da.MoTa,
            da.NgayBatDau,
            da.NgayKetThuc,
            da.TrangThai
        }).ToList();

            var lines = new List<string>();

            // Header với độ rộng cố định
            lines.Add(string.Format("{0,-5} {1,-30} {2,-50} {3,-12} {4,-12} {5,-15}",
                "Mã DA", "Tên dự án", "Mô tả", "Ngày bắt đầu", "Ngày kết thúc", "Trạng thái"));

            foreach (var da in duAns)
            {
                lines.Add(string.Format("{0,-5} {1,-30} {2,-50} {3,-12} {4,-12} {5,-15}",
                    da.MaDa,
                    da.TenDuAn.Length > 30 ? da.TenDuAn.Substring(0, 30) : da.TenDuAn,
                    da.MoTa.Length > 50 ? da.MoTa.Substring(0, 50) : da.MoTa,
                    da.NgayBatDau?.ToString("dd/MM/yyyy") ?? "",
                    da.NgayKetThuc?.ToString("dd/MM/yyyy") ?? "",
                    da.TrangThai));
            }

            var fileBytes = System.Text.Encoding.UTF8.GetBytes(string.Join(Environment.NewLine, lines));
            return File(fileBytes, "text/plain", "DanhSachDuAn.txt");
        }
    }
}

