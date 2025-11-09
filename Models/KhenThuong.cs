using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("KhenThuong")]
public partial class KhenThuong
{
    [Key]
    [Column("MaKT")]
    public int MaKt { get; set; }

    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(500)]
    public string LyDo { get; set; } = null!;

    [Column("NgayKT")]
    public DateOnly? NgayKt { get; set; }

    public int? NguoiLap { get; set; }

    [ForeignKey("MaTv")]
    [InverseProperty("KhenThuongs")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
