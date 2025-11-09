using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaHd", "MaTv")]
public partial class ThamGium
{
    [Key]
    [Column("MaHD")]
    public int MaHd { get; set; }

    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [Column("VaiTroTrongHD")]
    [StringLength(100)]
    public string? VaiTroTrongHd { get; set; }

    public bool? DiemDanh { get; set; }

    public double? DiemThuong { get; set; }

    [StringLength(500)]
    public string? DanhGia { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("ThamGia")]
    public virtual HoatDong MaHdNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("ThamGia")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
