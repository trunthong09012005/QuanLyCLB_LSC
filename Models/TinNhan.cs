using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("TinNhan")]
public partial class TinNhan
{
    [Key]
    [Column("MaTN")]
    public int MaTn { get; set; }

    public int MaNguoiGui { get; set; }

    public int MaNguoiNhan { get; set; }

    [StringLength(500)]
    public string NoiDung { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayGui { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaNguoiGui")]
    [InverseProperty("TinNhanMaNguoiGuiNavigations")]
    public virtual ThanhVien MaNguoiGuiNavigation { get; set; } = null!;

    [ForeignKey("MaNguoiNhan")]
    [InverseProperty("TinNhanMaNguoiNhanNavigations")]
    public virtual ThanhVien MaNguoiNhanNavigation { get; set; } = null!;
}
