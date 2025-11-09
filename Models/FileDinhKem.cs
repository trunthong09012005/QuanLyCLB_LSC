using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("FileDinhKem")]
public partial class FileDinhKem
{
    [Key]
    public int MaFile { get; set; }

    [StringLength(255)]
    public string TenFile { get; set; } = null!;

    [StringLength(500)]
    public string DuongDan { get; set; } = null!;

    [StringLength(50)]
    public string? LoaiFile { get; set; }

    [Column("MaDA")]
    public int? MaDa { get; set; }

    [Column("MaHD")]
    public int? MaHd { get; set; }

    [ForeignKey("MaDa")]
    [InverseProperty("FileDinhKems")]
    public virtual DuAn? MaDaNavigation { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("FileDinhKems")]
    public virtual HoatDong? MaHdNavigation { get; set; }
}
