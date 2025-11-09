using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaHd", "MaTv")]
[Table("HoatDong_ThanhVien")]
public partial class HoatDongThanhVien
{
    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(50)]
    public string? VaiTroThamGia { get; set; }

    [StringLength(200)]
    public string? KetQua { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("HoatDongThanhViens")]
    public virtual HoatDong MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("HoatDongThanhViens")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
