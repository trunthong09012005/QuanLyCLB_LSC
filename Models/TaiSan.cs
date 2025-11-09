using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("TaiSan")]
public partial class TaiSan
{
    [Key]
    [Column("MaTS")]
    public int MaTs { get; set; }

    [Column("TenTS")]
    [StringLength(255)]
    public string? TenTs { get; set; }

    public int? SoLuong { get; set; }

    [StringLength(50)]
    public string? DonViTinh { get; set; }

    [StringLength(100)]
    public string? TinhTrang { get; set; }

    public DateOnly? NgayNhap { get; set; }

    public int? NguoiQuanLy { get; set; }

    [StringLength(255)]
    public string? GhiChu { get; set; }

    [ForeignKey("NguoiQuanLy")]
    [InverseProperty("TaiSans")]
    public virtual ThanhVien? NguoiQuanLyNavigation { get; set; }
}
