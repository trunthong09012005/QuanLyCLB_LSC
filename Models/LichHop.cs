using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("LichHop")]
public partial class LichHop
{
    [Key]
    [Column("MaLH")]
    public int MaLh { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime NgayHop { get; set; }

    [StringLength(255)]
    public string? DiaDiem { get; set; }

    [StringLength(500)]
    public string? NoiDung { get; set; }

    public int? NguoiChuTri { get; set; }

    [StringLength(500)]
    public string? GhiChu { get; set; }

    [InverseProperty("MaLhNavigation")]
    public virtual ICollection<DiemDanhLichHop> DiemDanhLichHops { get; set; } = new List<DiemDanhLichHop>();

    [ForeignKey("NguoiChuTri")]
    [InverseProperty("LichHops")]
    public virtual ThanhVien? NguoiChuTriNavigation { get; set; }
}
