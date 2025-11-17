using System;
using System.Collections.Generic;

namespace QuanLyCLB_LSC.ViewModels
{
    public class UserDashboardViewModel
    {
        // Thông tin thành viên
        public int MaTV { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public string GioiTinh { get; set; }
        public string Lop { get; set; }
        public string Khoa { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string DiaChi { get; set; }
        public string VaiTro { get; set; }
        public DateTime? NgayThamGia { get; set; }

        public string TrangThai { get; set; }

        // Chức vụ và Ban
        public string TenChucVu { get; set; }
        public string TenBan { get; set; }

        // Điểm rèn luyện
        public int? DiemRenLuyen { get; set; }
        public string HocKy { get; set; }
        public string NamHoc { get; set; }

        // Thống kê
        public int TongThanhVien { get; set; }
        public int TongSuKien { get; set; }
        public int TongDuAn { get; set; }
        public int SoGiaiThuong { get; set; }

        // Hoạt động
        public List<HoatDongViewModel> DanhSachHoatDong { get; set; }

        // Khen thưởng
        public List<KhenThuongViewModel> DanhSachKhenThuong { get; set; }

        // Timeline (Lịch họp và Hoạt động sắp tới)
        public List<TimelineItemViewModel> DanhSachTimeline { get; set; }

        public UserDashboardViewModel()
        {
            DanhSachHoatDong = new List<HoatDongViewModel>();
            DanhSachKhenThuong = new List<KhenThuongViewModel>();
            DanhSachTimeline = new List<TimelineItemViewModel>();
        }
    }

    public class HoatDongViewModel
    {
        public int MaHD { get; set; }
        public string TenHD { get; set; }
        public DateTime? NgayToChuc { get; set; }
        public string DiaDiem { get; set; }
        public string MoTa { get; set; }
        public string TenLoaiHD { get; set; }
        public string TrangThai { get; set; }
        public string HinhAnh { get; set; }
    }

    public class KhenThuongViewModel
    {
        public int MaKT { get; set; }
        public string LyDo { get; set; }
        public DateTime? NgayKT { get; set; } // Sửa
        public string LoaiKhenThuong { get; set; }
    }

    public class TimelineItemViewModel
    {
        public string Loai { get; set; }
        public int MaItem { get; set; }
        public string TieuDe { get; set; }
        public string NoiDung { get; set; }
        public DateTime? NgayDienRa { get; set; } // Sửa
        public string DiaDiem { get; set; }
        public string MucDoUuTien { get; set; }
        public string Icon { get; set; }
    }

}