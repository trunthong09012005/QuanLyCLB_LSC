using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyCLB_LSC.Models;
using QuanLyCLB_LSC.ViewModels;
using System.Linq;
using System.Text.Json;

namespace QuanLyCLB_LSC.Controllers
{
    public class ReportsController : Controller
    {
        private readonly QlClbLscContext _context;

        public ReportsController(QlClbLscContext context)
        {
            _context = context;
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
            
            // Members per month
            var membersData = Enumerable.Range(1, 12)
                .Select(m => allMembers.Count(tv => tv.NgayThamGia.HasValue && tv.NgayThamGia.Value.Month == m && tv.NgayThamGia.Value.Year == selectedYear))
                .ToList();
            if (membersData.All(v => v == 0))
            {
                membersData = Enumerable.Range(1, 12)
                    .Select(m => allMembers.Count(tv => tv.NgayThamGia.HasValue && tv.NgayThamGia.Value.Month == m))
                    .ToList();
            }

            // Activities per month
            var activitiesData = Enumerable.Range(1, 12)
                .Select(m => allActivities.Count(h => h.NgayToChuc.HasValue && h.NgayToChuc.Value.Month == m && h.NgayToChuc.Value.Year == selectedYear))
                .ToList();
            if (activitiesData.All(v => v == 0))
            {
                activitiesData = Enumerable.Range(1, 12)
                    .Select(m => allActivities.Count(h => h.NgayToChuc.HasValue && h.NgayToChuc.Value.Month == m))
                    .ToList();
            }

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

            if (financeIncome.All(v => v == 0) && financeExpense.All(v => v == 0))
            {
                financeIncome = Enumerable.Range(1, 12)
                    .Select(m => allThuChi.Where(t => t.NgayGd.HasValue && t.NgayGd.Value.Month == m && string.Equals(t.LoaiGd, "Thu", StringComparison.OrdinalIgnoreCase)).Sum(t => (decimal?)t.SoTien) ?? 0)
                    .ToList();
                financeExpense = Enumerable.Range(1, 12)
                    .Select(m => allThuChi.Where(t => t.NgayGd.HasValue && t.NgayGd.Value.Month == m && string.Equals(t.LoaiGd, "Chi", StringComparison.OrdinalIgnoreCase)).Sum(t => (decimal?)t.SoTien) ?? 0)
                    .ToList();
            }

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
        public IActionResult CategoryDetails(string category, string? search = null, string? filterType = null, int? filterStatus = null, int page = 1)
        {
            const int pageSize = 15;
            var categoryViewModel = new CategoryDetailsViewModel
            {
                Category = category,
                Search = search,
                FilterType = filterType,
                Page = page
            };

            switch (category.ToLower())
            {
                case "members":
                    var membersQuery = _context.ThanhViens.AsQueryable();
                    
                    if (!string.IsNullOrWhiteSpace(search))
                        membersQuery = membersQuery.Where(m => m.HoTen.Contains(search) || m.Email.Contains(search));
                    
                    if (!string.IsNullOrWhiteSpace(filterType))
                        membersQuery = membersQuery.Where(m => m.TrangThai == filterType);

                    var totalMembers = membersQuery.Count();
                    var members = membersQuery
                        .OrderByDescending(m => m.NgayThamGia)
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

                    var totalActivities = activitiesQuery.Count();
                    var activities = activitiesQuery
                        .Include(a => a.NguoiPhuTrachNavigation)
                        .Include(a => a.MaLoaiHdNavigation)
                        .OrderByDescending(a => a.NgayToChuc)
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

                    var totalProjects = projectsQuery.Count();
                    var projects = projectsQuery
                        .OrderByDescending(p => p.NgayBatDau)
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

                    var totalFinance = financeQuery.Count();
                    var finance = financeQuery
                        .Include(f => f.NguoiThucHienNavigation)
                        .Include(f => f.MaNguonNavigation)
                        .OrderByDescending(f => f.NgayGd)
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
    }
}