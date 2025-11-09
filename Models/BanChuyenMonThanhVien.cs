using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaBan", "MaTv")]
[Table("BanChuyenMon_ThanhVien")]
public partial class BanChuyenMonThanhVien
{
    [Key]
    public int MaBan { get; set; }

    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(100)]
    public string? VaiTro { get; set; }

    public DateOnly? NgayThamGiaBan { get; set; }

    [ForeignKey("MaBan")]
    [InverseProperty("BanChuyenMonThanhViens")]
    public virtual BanChuyenMon MaBanNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("BanChuyenMonThanhViens")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
