using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaTv", "MaDa")]
[Table("PhanCong")]
public partial class PhanCong
{
    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [Key]
    [Column("MaDA")]
    public int MaDa { get; set; }

    [StringLength(500)]
    public string? NhiemVu { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayPhanCong { get; set; }

    [ForeignKey("MaDa")]
    [InverseProperty("PhanCongs")]
    public virtual DuAn MaDaNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("PhanCongs")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
