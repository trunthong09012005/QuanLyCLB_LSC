using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace QuanLyCLB_LSC.ViewModels
{
    public class RegisterViewModel : IValidatableObject
    {
        // ===== THÔNG TIN ĐĂNG NHẬP =====

        [Required(ErrorMessage = "Tên đăng nhập không được để trống")]
        [StringLength(50, MinimumLength = 5, ErrorMessage = "Tên đăng nhập phải từ 5-50 ký tự")]
        [RegularExpression(@"^[a-zA-Z0-9_\.]+$", ErrorMessage = "Tên đăng nhập chỉ chứa chữ cái, số, dấu gạch dưới và dấu chấm")]
        [Display(Name = "Tên đăng nhập")]
        public string TenDN { get; set; } = null!;

        [Required(ErrorMessage = "Mật khẩu không được để trống")]
        [StringLength(255, MinimumLength = 8, ErrorMessage = "Mật khẩu phải từ 8-255 ký tự")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#])[A-Za-z\d@$!%*?&#]{8,}$",
            ErrorMessage = "Mật khẩu phải chứa ít nhất 1 chữ hoa, 1 chữ thường, 1 số và 1 ký tự đặc biệt")]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string MatKhau { get; set; } = null!;

        // ===== THÔNG TIN CÁ NHÂN =====

        [Required(ErrorMessage = "Họ tên không được để trống")]
        [StringLength(150, MinimumLength = 2, ErrorMessage = "Họ tên phải từ 2-150 ký tự")]
        [RegularExpression(@"^[a-zA-ZÀ-ỹ\s]+$", ErrorMessage = "Họ tên chỉ chứa chữ cái và khoảng trắng")]
        [Display(Name = "Họ tên")]
        public string HoTen { get; set; } = null!;

        [DataType(DataType.Date)]
        [Display(Name = "Ngày sinh")]
        [CustomValidation(typeof(RegisterViewModel), nameof(ValidateNgaySinh))]
        public DateTime? NgaySinh { get; set; }

        [StringLength(10)]
        [RegularExpression(@"^(Nam|Nữ|Khác)$", ErrorMessage = "Giới tính phải là Nam, Nữ hoặc Khác")]
        [Display(Name = "Giới tính")]
        public string? GioiTinh { get; set; }

        [Required(ErrorMessage = "Số điện thoại không được để trống")]
        [StringLength(15, MinimumLength = 10, ErrorMessage = "Số điện thoại phải từ 10-15 ký tự")]
        [RegularExpression(@"^(0|\+84)[3|5|7|8|9][0-9]{8}$", ErrorMessage = "Số điện thoại không hợp lệ (VD: 0912345678)")]
        [Display(Name = "Số điện thoại")]
        public string SDT { get; set; } = null!;

        [Required(ErrorMessage = "Email không được để trống")]
        [EmailAddress(ErrorMessage = "Email không đúng định dạng")]
        [StringLength(150)]
        [RegularExpression(@"^[\w\.-]+@[\w\.-]+\.\w+$", ErrorMessage = "Email không hợp lệ")]
        [Display(Name = "Email")]
        public string Email { get; set; } = null!;

        [StringLength(255, MinimumLength = 10, ErrorMessage = "Địa chỉ phải từ 10-255 ký tự")]
        [Display(Name = "Địa chỉ")]
        public string? DiaChi { get; set; }

        // ===== THÔNG TIN HỌC VẤN =====

        [StringLength(150)]
        [Display(Name = "Khoa")]
        public string? Khoa { get; set; }

        [StringLength(50)]
        [RegularExpression(@"^[A-Z]{2}\.\d{2}\.[A-Z]{2,6}$", ErrorMessage = "Lớp không đúng định dạng (VD: CQ.64.CNTT)")]
        [Display(Name = "Lớp")]
        public string? Lop { get; set; }

        [StringLength(100)]
        [RegularExpression(@"^(Thành viên|Ứng viên)$", ErrorMessage = "Vai trò phải là 'Thành viên' hoặc 'Ứng viên'")]
        [Display(Name = "Vai trò")]
        public string VaiTro { get; set; } = "Thành viên";

        // ===== CUSTOM VALIDATION =====

        public static ValidationResult? ValidateNgaySinh(DateTime? ngaySinh, ValidationContext context)
        {
            if (ngaySinh == null) return ValidationResult.Success;

            var today = DateTime.Today;
            var age = today.Year - ngaySinh.Value.Year;

            if (ngaySinh.Value > today.AddYears(-age)) age--;

            if (ngaySinh.Value > today)
                return new ValidationResult("Ngày sinh không thể là tương lai");

            if (age < 16)
                return new ValidationResult("Bạn phải trên 16 tuổi để đăng ký");

            if (age > 100)
                return new ValidationResult("Ngày sinh không hợp lệ");

            return ValidationResult.Success;
        }

        // ===== CROSS-FIELD VALIDATION =====

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var results = new List<ValidationResult>();

            // 1. Validate TenDN không chứa ký tự đặc biệt không cho phép
            if (!string.IsNullOrWhiteSpace(TenDN))
            {
                if (TenDN.Contains(" "))
                    results.Add(new ValidationResult("Tên đăng nhập không được chứa khoảng trắng", new[] { nameof(TenDN) }));

                if (TenDN.StartsWith(".") || TenDN.EndsWith("."))
                    results.Add(new ValidationResult("Tên đăng nhập không được bắt đầu hoặc kết thúc bằng dấu chấm", new[] { nameof(TenDN) }));
            }

            // 2. Validate HoTen không chứa số
            if (!string.IsNullOrWhiteSpace(HoTen))
            {
                if (Regex.IsMatch(HoTen, @"\d"))
                    results.Add(new ValidationResult("Họ tên không được chứa số", new[] { nameof(HoTen) }));

                if (HoTen.Length - HoTen.Replace(" ", "").Length > 5)
                    results.Add(new ValidationResult("Họ tên có quá nhiều khoảng trắng", new[] { nameof(HoTen) }));
            }

            // 3. Validate Email domain hợp lệ
            if (!string.IsNullOrWhiteSpace(Email))
            {
                var allowedDomains = new[] { "gmail.com", "yahoo.com", "outlook.com", "hotmail.com", "student.edu.vn" };
                var domain = Email.Split('@').LastOrDefault()?.ToLower();

                // Commented out for flexibility, uncomment if you want to restrict domains
                // if (!string.IsNullOrEmpty(domain) && !allowedDomains.Any(d => domain.EndsWith(d)))
                //     results.Add(new ValidationResult($"Email phải thuộc một trong các domain: {string.Join(", ", allowedDomains)}", new[] { nameof(Email) }));
            }

            // 4. Validate SDT không chứa ký tự đặc biệt
            if (!string.IsNullOrWhiteSpace(SDT))
            {
                if (SDT.Contains(" ") || SDT.Contains("-"))
                    results.Add(new ValidationResult("Số điện thoại không được chứa khoảng trắng hoặc dấu gạch ngang", new[] { nameof(SDT) }));
            }

            // 5. Validate Lop và Khoa phải cùng có hoặc cùng không có
            if (!string.IsNullOrWhiteSpace(Lop) && string.IsNullOrWhiteSpace(Khoa))
                results.Add(new ValidationResult("Vui lòng nhập Khoa khi đã nhập Lớp", new[] { nameof(Khoa) }));

            if (!string.IsNullOrWhiteSpace(Khoa) && string.IsNullOrWhiteSpace(Lop))
                results.Add(new ValidationResult("Vui lòng nhập Lớp khi đã nhập Khoa", new[] { nameof(Lop) }));

            return results;
        }
    }
}