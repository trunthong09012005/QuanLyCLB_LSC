using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("LichSuThaoTac")]
public partial class LichSuThaoTac
{
    [Key]
    [Column("MaLSTT")]
    public int MaLstt { get; set; }

    [Column("MaTV")]
    public int? MaTv { get; set; }

    [StringLength(100)]
    public string? TenBang { get; set; }

    [StringLength(50)]
    public string? LoaiThaoTac { get; set; }

    [StringLength(100)]
    public string? KhoaChinh { get; set; }

    [StringLength(500)]
    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayThucHien { get; set; }

    [ForeignKey("MaTv")]
    [InverseProperty("LichSuThaoTacs")]
    public virtual ThanhVien? MaTvNavigation { get; set; }
}
