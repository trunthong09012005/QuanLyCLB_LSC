using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("HoatDong")]
public partial class HoatDong
{
    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column("TenHD")]
    [StringLength(200)]
    public string TenHd { get; set; } = null!;

    public DateOnly? NgayToChuc { get; set; }

    [StringLength(255)]
    public string? DiaDiem { get; set; }

    [StringLength(500)]
    public string? MoTa { get; set; }

    [Column("MaLoaiHD")]
    public int? MaLoaiHd { get; set; }

    public int? NguoiPhuTrach { get; set; }

    [Column(TypeName = "money")]
    public decimal? KinhPhiDuKien { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<DangKyHoatDong> DangKyHoatDongs { get; set; } = new List<DangKyHoatDong>();

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<Feedback> Feedbacks { get; set; } = new List<Feedback>();

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<FileDinhKem> FileDinhKems { get; set; } = new List<FileDinhKem>();

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<HoatDongThanhVien> HoatDongThanhViens { get; set; } = new List<HoatDongThanhVien>();

    [ForeignKey("MaLoaiHd")]
    [InverseProperty("HoatDongs")]
    public virtual LoaiHoatDong? MaLoaiHdNavigation { get; set; }

    [ForeignKey("NguoiPhuTrach")]
    [InverseProperty("HoatDongs")]
    public virtual ThanhVien? NguoiPhuTrachNavigation { get; set; }

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<ThamGium> ThamGia { get; set; } = new List<ThamGium>();

    [InverseProperty("MaHdNavigation")]
    public virtual ICollection<ThuChi> ThuChis { get; set; } = new List<ThuChi>();
}
