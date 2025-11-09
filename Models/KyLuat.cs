using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("KyLuat")]
public partial class KyLuat
{
    [Key]
    [Column("MaKL")]
    public int MaKl { get; set; }

    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(500)]
    public string LyDo { get; set; } = null!;

    [Column("NgayKL")]
    public DateOnly? NgayKl { get; set; }

    public int? NguoiLap { get; set; }

    [ForeignKey("MaTv")]
    [InverseProperty("KyLuats")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
