using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("ThanhVien")]
[Index("HoTen", Name = "IX_ThanhVien_HoTen")]
[Index("Email", Name = "UQ_ThanhVien_Email", IsUnique = true)]
public partial class ThanhVien
{
    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(150)]
    public string HoTen { get; set; } = null!;

    public DateOnly? NgaySinh { get; set; }

    [StringLength(10)]
    public string? GioiTinh { get; set; }

    [StringLength(50)]
    public string? Lop { get; set; }

    [StringLength(150)]
    public string? Khoa { get; set; }

    [Column("SDT")]
    [StringLength(15)]
    [Unicode(false)]
    public string? Sdt { get; set; }

    [StringLength(150)]
    public string? Email { get; set; }

    [StringLength(255)]
    public string? DiaChi { get; set; }

    [StringLength(100)]
    public string? VaiTro { get; set; }

    public DateOnly? NgayThamGia { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [Column("MaCV")]
    public int? MaCv { get; set; }

    public int? MaBan { get; set; }

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<BanChuyenMonThanhVien> BanChuyenMonThanhViens { get; set; } = new List<BanChuyenMonThanhVien>();

    [InverseProperty("TruongBanNavigation")]
    public virtual ICollection<BanChuyenMon> BanChuyenMons { get; set; } = new List<BanChuyenMon>();

    [InverseProperty("NguoiLapNavigation")]
    public virtual ICollection<BaoCao> BaoCaos { get; set; } = new List<BaoCao>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<DangKyHoatDong> DangKyHoatDongs { get; set; } = new List<DangKyHoatDong>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<DiemDanhLichHop> DiemDanhLichHops { get; set; } = new List<DiemDanhLichHop>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<DiemRenLuyen> DiemRenLuyens { get; set; } = new List<DiemRenLuyen>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<HoatDongThanhVien> HoatDongThanhViens { get; set; } = new List<HoatDongThanhVien>();

    [InverseProperty("NguoiPhuTrachNavigation")]
    public virtual ICollection<HoatDong> HoatDongs { get; set; } = new List<HoatDong>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<KhenThuong> KhenThuongs { get; set; } = new List<KhenThuong>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<KyLuat> KyLuats { get; set; } = new List<KyLuat>();

    [InverseProperty("NguoiChuTriNavigation")]
    public virtual ICollection<LichHop> LichHops { get; set; } = new List<LichHop>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<LichSuThaoTac> LichSuThaoTacs { get; set; } = new List<LichSuThaoTac>();

    [ForeignKey("MaBan")]
    [InverseProperty("ThanhViens")]
    public virtual BanChuyenMon? MaBanNavigation { get; set; }

    [ForeignKey("MaCv")]
    [InverseProperty("ThanhViens")]
    public virtual ChucVu? MaCvNavigation { get; set; }

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<PhanCong> PhanCongs { get; set; } = new List<PhanCong>();

    [InverseProperty("MaTvNavigation")]
    public virtual TaiKhoan? TaiKhoan { get; set; }

    [InverseProperty("NguoiQuanLyNavigation")]
    public virtual ICollection<TaiSan> TaiSans { get; set; } = new List<TaiSan>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<ThamGium> ThamGia { get; set; } = new List<ThamGium>();

    [InverseProperty("MaTvNavigation")]
    public virtual ICollection<ThanhVienKyNang> ThanhVienKyNangs { get; set; } = new List<ThanhVienKyNang>();

    [InverseProperty("NguoiDangNavigation")]
    public virtual ICollection<ThongBao> ThongBaos { get; set; } = new List<ThongBao>();

    [InverseProperty("NguoiThucHienNavigation")]
    public virtual ICollection<ThuChi> ThuChis { get; set; } = new List<ThuChi>();

    [InverseProperty("MaNguoiGuiNavigation")]
    public virtual ICollection<TinNhan> TinNhanMaNguoiGuiNavigations { get; set; } = new List<TinNhan>();

    [InverseProperty("MaNguoiNhanNavigation")]
    public virtual ICollection<TinNhan> TinNhanMaNguoiNhanNavigations { get; set; } = new List<TinNhan>();
}
