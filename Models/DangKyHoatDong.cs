using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaTv", "MaHd")]
[Table("DangKyHoatDong")]
public partial class DangKyHoatDong
{
    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianDangKy { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("DangKyHoatDongs")]
    public virtual HoatDong MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("DangKyHoatDongs")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
