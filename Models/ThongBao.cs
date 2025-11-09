using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("ThongBao")]
public partial class ThongBao
{
    [Key]
    [Column("MaTB")]
    public int MaTb { get; set; }

    [StringLength(255)]
    public string TieuDe { get; set; } = null!;

    public string? NoiDung { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayDang { get; set; }

    public int? NguoiDang { get; set; }

    [StringLength(100)]
    public string? DoiTuong { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    [ForeignKey("NguoiDang")]
    [InverseProperty("ThongBaos")]
    public virtual ThanhVien? NguoiDangNavigation { get; set; }
}
