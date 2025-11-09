using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("BaoCao")]
public partial class BaoCao
{
    [Key]
    [Column("MaBC")]
    public int MaBc { get; set; }

    [StringLength(255)]
    public string TieuDe { get; set; } = null!;

    [Column("LoaiBC")]
    [StringLength(100)]
    public string? LoaiBc { get; set; }

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayLap { get; set; }

    public int? NguoiLap { get; set; }

    [ForeignKey("NguoiLap")]
    [InverseProperty("BaoCaos")]
    public virtual ThanhVien? NguoiLapNavigation { get; set; }
}
