using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using System.Linq;
using System.Text.Json;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using QuanLyCLB_LSC.Services;
using System.Security.Claims;

namespace QuanLyCLB_LSC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly QlClbLscContext _context;
        private readonly IAuditService _audit;

        public ReportsController(QlClbLscContext context, IAuditService audit)
        {
            _context = context;
            _audit = audit;
        }

        // GET: Reports/Index - Báo cáo tổng thể dự án
        [HttpGet]
        public IActionResult Index(string category = "overview", int? year = null, string? search = null, string? filterType = null)
        {
            int selectedYear = year ?? DateTime.Now.Year;
            
            // Prepare data for charts
            var allMembers = _context.ThanhViens.AsEnumerable().ToList();
            var allActivities = _context.HoatDongs.AsEnumerable().ToList();
            var allProjects = _context.DuAns.AsEnumerable().ToList();
            var allThuChi = _context.ThuChis.AsEnumerable().ToList();

            // Chart labels
            var monthLabels = Enumerable.Range(1, 12).Select(m => $"Tháng {m}").ToList();
            
            // Members per month for selected year (keep zeros if none)
            var membersData = Enumerable.Range(1, 12)
                .Select(m => allMembers.Count(tv => tv.NgayThamGia.HasValue && tv.NgayThamGia.Value.Month == m && tv.NgayThamGia.Value.Year == selectedYear))
                .ToList();

            // Activities per month for selected year (keep zeros if none)
            var activitiesData = Enumerable.Range(1, 12)
                .Select(m => allActivities.Count(h => h.NgayToChuc.HasValue && h.NgayToChuc.Value.Month == m && h.NgayToChuc.Value.Year == selectedYear))
                .ToList();

            // Projects by status
            var projectStatuses = allProjects.Select(d => d.TrangThai ?? "Chưa xác định").Distinct().ToList();
            var projectsData = projectStatuses.Select(s => allProjects.Count(d => (d.TrangThai ?? "Chưa xác định") == s)).ToList();
            // Finance: income and expense per month
            var financeIncome = Enumerable.Range(1, 12)
                .Select(m => allThuChi.Where(t => t.NgayGd.HasValue && t.NgayGd.Value.Month == m && t.NgayGd.Value.Year == selectedYear && string.Equals(t.LoaiGd, "Thu", StringComparison.OrdinalIgnoreCase)).Sum(t => (decimal?)t.SoTien) ?? 0)
                .ToList();
            var financeExpense = Enumerable.Range(1, 12)
                .Select(m => allThuChi.Where(t => t.NgayGd.HasValue && t.NgayGd.Value.Month == m && t.NgayGd.Value.Year == selectedYear && string.Equals(t.LoaiGd, "Chi", StringComparison.OrdinalIgnoreCase)).Sum(t => (decimal?)t.SoTien) ?? 0)
                .ToList();

            // Keep finance arrays as computed for the selected year (zeros if no data)

            // Statistics
            var stats = new ReportStatisticsViewModel
            {
                TongBaoCao = _context.BaoCaos.Count(),
                BaoCaoThangNay = _context.BaoCaos.Count(bc => bc.NgayLap.HasValue && bc.NgayLap.Value.Month == DateTime.Now.Month && bc.NgayLap.Value.Year == DateTime.Now.Year),
                BaoCaoNamNay = _context.BaoCaos.Count(bc => bc.NgayLap.HasValue && bc.NgayLap.Value.Year == DateTime.Now.Year),
                DanhSachLoai = _context.BaoCaos.Where(b => !string.IsNullOrEmpty(b.LoaiBc)).Select(b => b.LoaiBc).Distinct().ToList()
            };

            // Serialize chart data
            ViewBag.MembersChartLabels = JsonSerializer.Serialize(monthLabels);
            ViewBag.MembersChartData = JsonSerializer.Serialize(membersData);
            ViewBag.ActivitiesChartLabels = JsonSerializer.Serialize(monthLabels);
            ViewBag.ActivitiesChartData = JsonSerializer.Serialize(activitiesData);
            ViewBag.ProjectsChartLabels = JsonSerializer.Serialize(projectStatuses);
            ViewBag.ProjectsChartData = JsonSerializer.Serialize(projectsData);
            ViewBag.FinanceChartLabels = JsonSerializer.Serialize(monthLabels);
            ViewBag.FinanceChartIncome = JsonSerializer.Serialize(financeIncome);
            ViewBag.FinanceChartExpense = JsonSerializer.Serialize(financeExpense);
            
            ViewBag.SelectedYear = selectedYear;
            ViewBag.SelectedCategory = category;

            return View(stats);
        }

        // GET: Reports/CategoryDetails - Chi tiết danh mục
        [HttpGet]
        public IActionResult CategoryDetails(string category, string? search = null, string? filterType = null, int? filterStatus = null, string? sortBy = null, string? sortDir = null, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(category))
            {
                // If no category provided, redirect to Reports index to avoid null reference
                return RedirectToAction("Index");
            }

            const int pageSize = 15;
            var categoryViewModel = new CategoryDetailsViewModel
            {
                Category = category,
                Search = search,
                FilterType = filterType,
                Page = page
            };

            // expose current sorting to the view
            ViewBag.SortBy = sortBy;
            ViewBag.SortDir = sortDir;

            switch (category.ToLower())
            {
                case "members":
                    var membersQuery = _context.ThanhViens.AsQueryable();
                    
                    if (!string.IsNullOrWhiteSpace(search))
                        membersQuery = membersQuery.Where(m => m.HoTen.Contains(search) || m.Email.Contains(search));
                    
                    if (!string.IsNullOrWhiteSpace(filterType))
                        membersQuery = membersQuery.Where(m => m.TrangThai == filterType);

                    // Apply sorting
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                        membersQuery = sortBy.ToLower() switch
                        {
                            "hoten" => asc ? membersQuery.OrderBy(m => m.HoTen) : membersQuery.OrderByDescending(m => m.HoTen),
                            "email" => asc ? membersQuery.OrderBy(m => m.Email) : membersQuery.OrderByDescending(m => m.Email),
                            "ngaythamgia" => asc ? membersQuery.OrderBy(m => m.NgayThamGia) : membersQuery.OrderByDescending(m => m.NgayThamGia),
                            _ => asc ? membersQuery.OrderBy(m => m.MaTv) : membersQuery.OrderByDescending(m => m.MaTv),
                        };
                    }
                    else
                    {
                        membersQuery = membersQuery.OrderByDescending(m => m.NgayThamGia);
                    }

                    var totalMembers = membersQuery.Count();
                    var members = membersQuery
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    categoryViewModel.Members = members;
                    categoryViewModel.TotalCount = totalMembers;
                    categoryViewModel.TotalPages = (int)Math.Ceiling(totalMembers / (double)pageSize);
                    break;

                case "activities":
                    var activitiesQuery = _context.HoatDongs.AsQueryable();
                    
                    if (!string.IsNullOrWhiteSpace(search))
                        activitiesQuery = activitiesQuery.Where(a => a.TenHd.Contains(search) || a.MoTa.Contains(search));
                    
                    if (!string.IsNullOrWhiteSpace(filterType))
                        activitiesQuery = activitiesQuery.Where(a => a.TrangThai == filterType);

                    // Sorting
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                        activitiesQuery = sortBy.ToLower() switch
                        {
                            "tenhd" => asc ? activitiesQuery.OrderBy(a => a.TenHd) : activitiesQuery.OrderByDescending(a => a.TenHd),
                            "ngaytochuc" => asc ? activitiesQuery.OrderBy(a => a.NgayToChuc) : activitiesQuery.OrderByDescending(a => a.NgayToChuc),
                            "trangthai" => asc ? activitiesQuery.OrderBy(a => a.TrangThai) : activitiesQuery.OrderByDescending(a => a.TrangThai),
                            _ => asc ? activitiesQuery.OrderBy(a => a.MaHd) : activitiesQuery.OrderByDescending(a => a.MaHd),
                        };
                    }
                    else
                    {
                        activitiesQuery = activitiesQuery.OrderByDescending(a => a.NgayToChuc);
                    }

                    var totalActivities = activitiesQuery.Count();
                    var activities = activitiesQuery
                        .Include(a => a.NguoiPhuTrachNavigation)
                        .Include(a => a.MaLoaiHdNavigation)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    categoryViewModel.Activities = activities;
                    categoryViewModel.TotalCount = totalActivities;
                    categoryViewModel.TotalPages = (int)Math.Ceiling(totalActivities / (double)pageSize);
                    break;

                case "projects":
                    var projectsQuery = _context.DuAns.AsQueryable();
                    
                    if (!string.IsNullOrWhiteSpace(search))
                        projectsQuery = projectsQuery.Where(p => p.TenDuAn.Contains(search) || p.MoTa.Contains(search));
                    
                    if (!string.IsNullOrWhiteSpace(filterType))
                        projectsQuery = projectsQuery.Where(p => p.TrangThai == filterType);

                    // Sorting
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                        projectsQuery = sortBy.ToLower() switch
                        {
                            "tenduan" => asc ? projectsQuery.OrderBy(p => p.TenDuAn) : projectsQuery.OrderByDescending(p => p.TenDuAn),
                            "ngaybatdau" => asc ? projectsQuery.OrderBy(p => p.NgayBatDau) : projectsQuery.OrderByDescending(p => p.NgayBatDau),
                            "trangthai" => asc ? projectsQuery.OrderBy(p => p.TrangThai) : projectsQuery.OrderByDescending(p => p.TrangThai),
                            _ => asc ? projectsQuery.OrderBy(p => p.MaDa) : projectsQuery.OrderByDescending(p => p.MaDa),
                        };
                    }
                    else
                    {
                        projectsQuery = projectsQuery.OrderByDescending(p => p.NgayBatDau);
                    }

                    var totalProjects = projectsQuery.Count();
                    var projects = projectsQuery
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    categoryViewModel.Projects = projects;
                    categoryViewModel.TotalCount = totalProjects;
                    categoryViewModel.TotalPages = (int)Math.Ceiling(totalProjects / (double)pageSize);
                    break;

                case "finance":
                    var financeQuery = _context.ThuChis.AsQueryable();
                    
                    if (!string.IsNullOrWhiteSpace(search))
                        financeQuery = financeQuery.Where(f => f.NoiDung.Contains(search));
                    
                    if (!string.IsNullOrWhiteSpace(filterType))
                        financeQuery = financeQuery.Where(f => f.LoaiGd == filterType);

                    // Sorting
                    if (!string.IsNullOrWhiteSpace(sortBy))
                    {
                        bool asc = string.Equals(sortDir, "asc", StringComparison.OrdinalIgnoreCase);
                        financeQuery = sortBy.ToLower() switch
                        {
                            "sotien" => asc ? financeQuery.OrderBy(f => f.SoTien) : financeQuery.OrderByDescending(f => f.SoTien),
                            "ngaygd" => asc ? financeQuery.OrderBy(f => f.NgayGd) : financeQuery.OrderByDescending(f => f.NgayGd),
                            _ => asc ? financeQuery.OrderBy(f => f.MaGd) : financeQuery.OrderByDescending(f => f.MaGd),
                        };
                    }
                    else
                    {
                        financeQuery = financeQuery.OrderByDescending(f => f.NgayGd);
                    }

                    var totalFinance = financeQuery.Count();
                    var finance = financeQuery
                        .Include(f => f.NguoiThucHienNavigation)
                        .Include(f => f.MaNguonNavigation)
                        .Skip((page - 1) * pageSize)
                        .Take(pageSize)
                        .ToList();

                    categoryViewModel.ThuChis = finance;
                    categoryViewModel.TotalCount = totalFinance;
                    categoryViewModel.TotalPages = (int)Math.Ceiling(totalFinance / (double)pageSize);
                    break;

                default:
                    return NotFound();
            }

            return View(categoryViewModel);
        }

        // GET: Reports/Details - Xem chi tiết báo cáo
        [HttpGet]
        public IActionResult Details(int id)
        {
            var baoCao = _context.BaoCaos
                .Include(bc => bc.NguoiLapNavigation)
                .FirstOrDefault(bc => bc.MaBc == id);

            if (baoCao == null)
                return NotFound();

            var model = new ReportDetailViewModel
            {
                MaBc = baoCao.MaBc,
                TieuDe = baoCao.TieuDe,
                LoaiBc = baoCao.LoaiBc,
                NoiDung = baoCao.NoiDung,
                NgayLap = baoCao.NgayLap,
                Thang = baoCao.NgayLap?.Month,
                Nam = baoCao.NgayLap?.Year,
                NguoiLap = baoCao.NguoiLap,
                NguoiLapTen = baoCao.NguoiLapNavigation?.HoTen
            };

            return View(model);
        }

        // GET: Reports/ExportFinancePdf - Xuất khẩu PDF giao dịch tài chính
        [HttpGet]
        public IActionResult ExportFinancePdf(int year)
        {
            // Lấy dữ liệu giao dịch tài chính theo năm
            var financeData = _context.ThuChis
                .Include(f => f.NguoiThucHienNavigation)
                .Include(f => f.MaNguonNavigation)
                .Where(f => f.NgayGd.HasValue && f.NgayGd.Value.Year == year)
                .ToList();

            // Tạo tài liệu PDF mới
            using (var pdfDocument = new PdfDocument())
            {
                pdfDocument.Info.Title = $"Báo cáo tài chính năm {year}";
                var page = pdfDocument.AddPage();
                page.Size = PdfSharpCore.PageSize.A4;
                page.Orientation = PdfSharpCore.PageOrientation.Portrait;

                // Tạo đối tượng vẽ
                XGraphics gfx = XGraphics.FromPdfPage(page);

                // Thiết lập font chữ
                var font = new XFont("Arial", 12, XFontStyle.Regular);

                // Xuất tiêu đề
                gfx.DrawString($"Báo cáo tài chính năm {year}", new XFont("Arial", 16, XFontStyle.Bold), XBrushes.Black, new XRect(0, 20, page.Width, 40), XStringFormats.Center);

                // Xuất tiêu đề bảng
                gfx.DrawString("Ngày giao dịch", font, XBrushes.Black, 40, 80);
                gfx.DrawString("Nội dung", font, XBrushes.Black, 150, 80);
                gfx.DrawString("Số tiền", font, XBrushes.Black, 350, 80);
                gfx.DrawString("Người thực hiện", font, XBrushes.Black, 450, 80);

                // Xuất dữ liệu
                int rowIndex = 0;
                foreach (var item in financeData)
                {
                    rowIndex++;
                    gfx.DrawString(item.NgayGd.Value.ToString("dd/MM/yyyy"), font, XBrushes.Black, 40, 80 + rowIndex * 20);
                    gfx.DrawString(item.NoiDung, font, XBrushes.Black, 150, 80 + rowIndex * 20);
                    gfx.DrawString(item.SoTien.ToString("N0"), font, XBrushes.Black, 350, 80 + rowIndex * 20);
                    gfx.DrawString(item.NguoiThucHienNavigation?.HoTen, font, XBrushes.Black, 450, 80 + rowIndex * 20);
                }

                // Lưu tài liệu PDF vào bộ nhớ tạm
                using (var stream = new System.IO.MemoryStream())
                {
                    pdfDocument.Save(stream, false);
                    var fileName = $"BaoCaoTaiChinh_{year}.pdf";

                    // audit
                    int? userId = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userId = parsed;
                    }
                    _audit.LogAsync(userId, "BaoCao", "Xuất PDF", $"FinanceYear={year}", $"Xuất báo cáo tài chính năm {year}").ConfigureAwait(false);

                    return File(stream.ToArray(), "application/pdf", fileName);
                }
            }
        }

        // GET: Reports/ExportCategoryPdf - Xuất PDF cho từng danh mục (members, activities, projects, finance)
        [HttpGet]
        public IActionResult ExportCategoryPdf(string category, string? search = null, string? filterType = null, int? year = null)
        {
            // normalize
            category = (category ?? string.Empty).ToLower();
            // Fetch data depending on category
            switch (category)
            {
                case "members":
                    var membersQuery = _context.ThanhViens.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(search))
                        membersQuery = membersQuery.Where(m => m.HoTen.Contains(search) || m.Email.Contains(search));
                    if (!string.IsNullOrWhiteSpace(filterType))
                        membersQuery = membersQuery.Where(m => m.TrangThai == filterType);
                    var members = membersQuery.OrderByDescending(m => m.NgayThamGia).ToList();

                    // audit
                    int? userIdMembers = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userIdMembers = parsed;
                    }
                    _audit.LogAsync(userIdMembers, "BaoCao", "Xuất PDF", $"Category=members", $"Xuất báo cáo Members (count={members.Count})").ConfigureAwait(false);

                    return GenerateMembersPdf(members, category);

                case "activities":
                    var activitiesQuery = _context.HoatDongs.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(search))
                        activitiesQuery = activitiesQuery.Where(a => a.TenHd.Contains(search) || a.MoTa.Contains(search));
                    if (!string.IsNullOrWhiteSpace(filterType))
                        activitiesQuery = activitiesQuery.Where(a => a.TrangThai == filterType);
                    var activities = activitiesQuery
                        .Include(a => a.NguoiPhuTrachNavigation)
                        .OrderByDescending(a => a.NgayToChuc)
                        .ToList();

                    int? userIdActivities = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userIdActivities = parsed;
                    }
                    _audit.LogAsync(userIdActivities, "BaoCao", "Xuất PDF", $"Category=activities", $"Xuất báo cáo Activities (count={activities.Count})").ConfigureAwait(false);

                    return GenerateActivitiesPdf(activities, category);

                case "projects":
                    var projectsQuery = _context.DuAns.AsQueryable();
                    if (!string.IsNullOrWhiteSpace(search))
                        projectsQuery = projectsQuery.Where(p => p.TenDuAn.Contains(search) || p.MoTa.Contains(search));
                    if (!string.IsNullOrWhiteSpace(filterType))
                        projectsQuery = projectsQuery.Where(p => p.TrangThai == filterType);
                    var projects = projectsQuery.OrderByDescending(p => p.NgayBatDau).ToList();

                    int? userIdProjects = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userIdProjects = parsed;
                    }
                    _audit.LogAsync(userIdProjects, "BaoCao", "Xuất PDF", $"Category=projects", $"Xuất báo cáo Projects (count={projects.Count})").ConfigureAwait(false);

                    return GenerateProjectsPdf(projects, category);

                case "finance":
                    var yr = year ?? DateTime.Now.Year;
                    var financeData = _context.ThuChis
                        .Include(f => f.NguoiThucHienNavigation)
                        .Include(f => f.MaNguonNavigation)
                        .Where(f => f.NgayGd.HasValue && f.NgayGd.Value.Year == yr)
                        .ToList();

                    int? userIdFinance = null;
                    if (User?.Identity?.IsAuthenticated == true)
                    {
                        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        if (int.TryParse(idClaim, out var parsed)) userIdFinance = parsed;
                    }
                    _audit.LogAsync(userIdFinance, "BaoCao", "Xuất PDF", $"Category=finance;Year={yr}", $"Xuất báo cáo Finance (count={financeData.Count})").ConfigureAwait(false);

                    return GenerateFinancePdf(financeData, yr);

                default:
                    return BadRequest("Danh mục không hợp lệ");
            }
        }

        // Helper: generate members PDF
        private IActionResult GenerateMembersPdf(List<ThanhVien> members, string category)
        {
            using var pdf = new PdfDocument();
            pdf.Info.Title = "Báo cáo Thành viên";

            var page = pdf.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);
            var fontHeader = new XFont("Arial", 14, XFontStyle.Bold);
            var font = new XFont("Arial", 10, XFontStyle.Regular);

            gfx.DrawString("Báo cáo Thành viên", fontHeader, XBrushes.Black, new XRect(0, 20, page.Width, 30), XStringFormats.Center);

            double marginLeft = 40;
            double y = 60;
            double lineHeight = 18;

            // Table header
            gfx.DrawString("Mã", font, XBrushes.Black, marginLeft, y);
            gfx.DrawString("Họ tên", font, XBrushes.Black, marginLeft + 50, y);
            gfx.DrawString("Email", font, XBrushes.Black, marginLeft + 250, y);
            gfx.DrawString("SDT", font, XBrushes.Black, marginLeft + 420, y);
            y += lineHeight;

            foreach (var m in members)
            {
                if (y > page.Height - 60)
                {
                    page = pdf.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(m.MaTv.ToString(), font, XBrushes.Black, marginLeft, y);
                gfx.DrawString(m.HoTen ?? string.Empty, font, XBrushes.Black, marginLeft + 50, y);
                gfx.DrawString(m.Email ?? string.Empty, font, XBrushes.Black, marginLeft + 250, y);
                gfx.DrawString(m.Sdt ?? string.Empty, font, XBrushes.Black, marginLeft + 420, y);
                y += lineHeight;
            }

            using var ms = new System.IO.MemoryStream();
            pdf.Save(ms, false);
            return File(ms.ToArray(), "application/pdf", "BaoCao_Members.pdf");
        }

        // Helper: generate activities PDF
        private IActionResult GenerateActivitiesPdf(List<HoatDong> activities, string category)
        {
            using var pdf = new PdfDocument();
            pdf.Info.Title = "Báo cáo Hoạt động";

            var page = pdf.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);
            var fontHeader = new XFont("Arial", 14, XFontStyle.Bold);
            var font = new XFont("Arial", 10, XFontStyle.Regular);

            gfx.DrawString("Báo cáo Hoạt động", fontHeader, XBrushes.Black, new XRect(0, 20, page.Width, 30), XStringFormats.Center);

            double marginLeft = 40;
            double y = 60;
            double lineHeight = 18;

            gfx.DrawString("Mã", font, XBrushes.Black, marginLeft, y);
            gfx.DrawString("Tên", font, XBrushes.Black, marginLeft + 50, y);
            gfx.DrawString("Ngày", font, XBrushes.Black, marginLeft + 300, y);
            gfx.DrawString("Địa điểm", font, XBrushes.Black, marginLeft + 380, y);
            y += lineHeight;

            foreach (var a in activities)
            {
                if (y > page.Height - 60)
                {
                    page = pdf.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(a.MaHd.ToString(), font, XBrushes.Black, marginLeft, y);
                gfx.DrawString(a.TenHd ?? string.Empty, font, XBrushes.Black, marginLeft + 50, y);
                gfx.DrawString(a.NgayToChuc?.ToString("dd/MM/yyyy") ?? string.Empty, font, XBrushes.Black, marginLeft + 300, y);
                gfx.DrawString(a.DiaDiem ?? string.Empty, font, XBrushes.Black, marginLeft + 380, y);
                y += lineHeight;
            }

            using var ms = new System.IO.MemoryStream();
            pdf.Save(ms, false);
            return File(ms.ToArray(), "application/pdf", "BaoCao_Activities.pdf");
        }

        // Helper: generate projects PDF
        private IActionResult GenerateProjectsPdf(List<DuAn> projects, string category)
        {
            using var pdf = new PdfDocument();
            pdf.Info.Title = "Báo cáo Dự án";

            var page = pdf.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);
            var fontHeader = new XFont("Arial", 14, XFontStyle.Bold);
            var font = new XFont("Arial", 10, XFontStyle.Regular);

            gfx.DrawString("Báo cáo Dự án", fontHeader, XBrushes.Black, new XRect(0, 20, page.Width, 30), XStringFormats.Center);

            double marginLeft = 40;
            double y = 60;
            double lineHeight = 18;

            gfx.DrawString("Mã", font, XBrushes.Black, marginLeft, y);
            gfx.DrawString("Tên dự án", font, XBrushes.Black, marginLeft + 50, y);
            gfx.DrawString("Trạng thái", font, XBrushes.Black, marginLeft + 380, y);
            y += lineHeight;

            foreach (var p in projects)
            {
                if (y > page.Height - 60)
                {
                    page = pdf.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(p.MaDa.ToString(), font, XBrushes.Black, marginLeft, y);
                gfx.DrawString(p.TenDuAn ?? string.Empty, font, XBrushes.Black, marginLeft + 50, y);
                gfx.DrawString(p.TrangThai ?? string.Empty, font, XBrushes.Black, marginLeft + 380, y);
                y += lineHeight;
            }

            using var ms = new System.IO.MemoryStream();
            pdf.Save(ms, false);
            return File(ms.ToArray(), "application/pdf", "BaoCao_Projects.pdf");
        }

        // Helper: generate finance PDF (reuse previous logic but with pagination)
        private IActionResult GenerateFinancePdf(List<ThuChi> financeData, int year)
        {
            using var pdf = new PdfDocument();
            pdf.Info.Title = $"Báo cáo tài chính năm {year}";

            var page = pdf.AddPage();
            page.Size = PdfSharpCore.PageSize.A4;
            var gfx = XGraphics.FromPdfPage(page);
            var fontHeader = new XFont("Arial", 14, XFontStyle.Bold);
            var font = new XFont("Arial", 10, XFontStyle.Regular);

            gfx.DrawString($"Báo cáo tài chính năm {year}", fontHeader, XBrushes.Black, new XRect(0, 20, page.Width, 30), XStringFormats.Center);

            double marginLeft = 40;
            double y = 60;
            double lineHeight = 18;

            gfx.DrawString("Ngày", font, XBrushes.Black, marginLeft, y);
            gfx.DrawString("Nội dung", font, XBrushes.Black, marginLeft + 80, y);
            gfx.DrawString("Số tiền", font, XBrushes.Black, marginLeft + 350, y);
            gfx.DrawString("Người thực hiện", font, XBrushes.Black, marginLeft + 430, y);
            y += lineHeight;

            foreach (var item in financeData)
            {
                if (y > page.Height - 60)
                {
                    page = pdf.AddPage();
                    page.Size = PdfSharpCore.PageSize.A4;
                    gfx = XGraphics.FromPdfPage(page);
                    y = 40;
                }

                gfx.DrawString(item.NgayGd?.ToString("dd/MM/yyyy") ?? string.Empty, font, XBrushes.Black, marginLeft, y);
                gfx.DrawString(item.NoiDung ?? string.Empty, font, XBrushes.Black, marginLeft + 80, y);
                gfx.DrawString(item.SoTien.ToString("N0"), font, XBrushes.Black, marginLeft + 350, y);
                gfx.DrawString(item.NguoiThucHienNavigation?.HoTen ?? string.Empty, font, XBrushes.Black, marginLeft + 430, y);
                y += lineHeight;
            }

            using var ms = new System.IO.MemoryStream();
            pdf.Save(ms, false);
            var fileName = $"BaoCaoTaiChinh_{year}.pdf";
            return File(ms.ToArray(), "application/pdf", fileName);
        }
    }
}