using System;
using System.ComponentModel.DataAnnotations;
using QuanLyCLB_LSC.Models;

namespace QuanLyCLB_LSC.ViewModels
{
    /// <summary>
    /// ViewModel cho báo cáo - danh sách t?t c? báo cáo v?i b? l?c
    /// </summary>
    public class ReportListViewModel
    {
        public int MaBc { get; set; }
        public string TieuDe { get; set; } = null!;
        public string? LoaiBc { get; set; }
        public DateTime? NgayLap { get; set; }
        public string? NguoiLapTen { get; set; }
        public int? NguoiLap { get; set; }
    }

    /// <summary>
    /// ViewModel cho chi ti?t báo cáo
    /// </summary>
    public class ReportDetailViewModel
    {
        [Required(ErrorMessage = "Tiêu ?? báo cáo b?t bu?c")]
        [StringLength(255, MinimumLength = 10, ErrorMessage = "Tiêu ?? t? 10-255 ký t?")]
        public string TieuDe { get; set; } = null!;

        [Required(ErrorMessage = "Lo?i báo cáo b?t bu?c")]
        [StringLength(100)]
        public string LoaiBc { get; set; } = null!;

        [Required(ErrorMessage = "N?i dung báo cáo b?t bu?c")]
        [StringLength(4000, MinimumLength = 50, ErrorMessage = "N?i dung t? 50-4000 ký t?")]
        public string NoiDung { get; set; } = null!;

        [Display(Name = "K?t lu?n")]
        [StringLength(2000)]
        public string? KetLuan { get; set; }

        [Display(Name = "?ánh giá chung")]
        [StringLength(2000)]
        public string? DanhGia { get; set; }

        [Display(Name = "Ngày l?p")]
        [DataType(DataType.Date)]
        public DateTime? NgayLap { get; set; }

        [Display(Name = "Tháng")]
        public int? Thang { get; set; }

        [Display(Name = "N?m")]
        public int? Nam { get; set; }

        public int? NguoiLap { get; set; }
        public string? NguoiLapTen { get; set; }
        public int MaBc { get; set; }
    }

    /// <summary>
    /// ViewModel cho b? l?c báo cáo
    /// </summary>
    public class ReportFilterViewModel
    {
        [Display(Name = "T? khóa tìm ki?m")]
        public string? SearchKeyword { get; set; }

        [Display(Name = "Lo?i báo cáo")]
        public string? LoaiBc { get; set; }

        [Display(Name = "Tháng")]
        public int? Thang { get; set; }

        [Display(Name = "N?m")]
        public int? Nam { get; set; }

        [Display(Name = "Ng??i l?p")]
        public int? NguoiLap { get; set; }

        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }

    /// <summary>
    /// ViewModel cho th?ng kê báo cáo
    /// </summary>
    public class ReportStatisticsViewModel
    {
        public int TongBaoCao { get; set; }
        public int BaoCaoThangNay { get; set; }
        public int BaoCaoNamNay { get; set; }
        public Dictionary<string, int> BaoCaoTheoLoai { get; set; } = new();
        public Dictionary<string, int> BaoCaoTheoThang { get; set; } = new();
        public List<string> DanhSachLoai { get; set; } = new();
    }

    /// <summary>
    /// ViewModel k?t h?p cho trang báo cáo
    /// </summary>
    public class ReportPageViewModel
    {
        public List<ReportListViewModel> BaoCaos { get; set; } = new();
        public ReportFilterViewModel Filter { get; set; } = new();
        public ReportStatisticsViewModel Statistics { get; set; } = new();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<ThanhVien> DanhSachNguoiLap { get; set; } = new();
    }

    /// <summary>
    /// ViewModel cho chi ti?t danh m?c báo cáo
    /// </summary>
    public class CategoryDetailsViewModel
    {
        public string Category { get; set; } = null!;
        public string? Search { get; set; }
        public string? FilterType { get; set; }
        public int Page { get; set; } = 1;
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        // Category-specific data
        public List<ThanhVien> Members { get; set; } = new();
        public List<HoatDong> Activities { get; set; } = new();
        public List<DuAn> Projects { get; set; } = new();
        public List<ThuChi> ThuChis { get; set; } = new();
    }
}
