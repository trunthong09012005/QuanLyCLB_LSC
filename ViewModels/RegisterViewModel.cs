using System;
using System.ComponentModel.DataAnnotations;

namespace QuanLyCLB_LSC.ViewModels
{
    public class RegisterViewModel
    {
        // ===== Thông tin đăng nhập =====
        [Required(ErrorMessage = "Tên đăng nhập bắt buộc")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Tên đăng nhập từ 3-50 ký tự")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDN { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu bắt buộc")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Mật khẩu từ 6-255 ký tự")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = null!;

        // ===== Thông tin thành viên (ThanhVien) =====
        [Required(ErrorMessage = "Họ tên bắt buộc")]
        [StringLength(150)]
        [Display(Name = "Họ tên")]
        public string? HoTen { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [StringLength(50)]
        [Display(Name = "Lớp")]
        public string? Lop { get; set; }

        [StringLength(150)]
        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }

        [StringLength(15)]
        [Display(Name = "Số điện thoại")]
        public string? SDT { get; set; }

        [Required(ErrorMessage = "Email bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(150)]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [StringLength(255)]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        [StringLength(100)]
        [Display(Name = "Vai trò")]
        public string? VaiTro { get; set; } = "Thành viên"; // default
    }
}
