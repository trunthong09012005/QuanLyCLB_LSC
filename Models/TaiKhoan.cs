using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("TaiKhoan")]
[Index("MaTv", Name = "UQ__TaiKhoan__2725007AA68ECBF1", IsUnique = true)]
[Index("TenDn", Name = "UQ__TaiKhoan__4CF96558228D18E0", IsUnique = true)]
public partial class TaiKhoan
{
    [Key]
    [Column("MaTK")]
    public int MaTk { get; set; }

    [Column("TenDN")]
    [StringLength(50)]
    public string TenDn { get; set; } = null!;

    [StringLength(255)]
    public string MatKhau { get; set; } = null!;

    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(50)]
    public string QuyenHan { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayTao { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaTv")]
    [InverseProperty("TaiKhoan")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
#if true

#endif