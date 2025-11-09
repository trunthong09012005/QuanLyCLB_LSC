using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("DuAn")]
public partial class DuAn
{
    [Key]
    [Column("MaDA")]
    public int MaDa { get; set; }

    [StringLength(200)]
    public string TenDuAn { get; set; } = null!;

    [StringLength(500)]
    public string? MoTa { get; set; }

    public DateOnly? NgayBatDau { get; set; }

    public DateOnly? NgayKetThuc { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [InverseProperty("MaDaNavigation")]
    public virtual ICollection<FileDinhKem> FileDinhKems { get; set; } = new List<FileDinhKem>();

    [InverseProperty("MaDaNavigation")]
    public virtual ICollection<PhanCong> PhanCongs { get; set; } = new List<PhanCong>();
}
