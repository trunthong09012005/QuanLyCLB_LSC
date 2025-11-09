using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaTv", "MaKn")]
[Table("ThanhVien_KyNang")]
public partial class ThanhVienKyNang
{
    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [Key]
    [Column("MaKN")]
    public int MaKn { get; set; }

    public double? Diem { get; set; }

    [StringLength(50)]
    public string? CapDoHienTai { get; set; }

    public DateOnly? NgayCapNhat { get; set; }

    [ForeignKey("MaKn")]
    [InverseProperty("ThanhVienKyNangs")]
    public virtual KyNang MaKnNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("ThanhVienKyNangs")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
