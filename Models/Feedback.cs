using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("Feedback")]
public partial class Feedback
{
    [Key]
    [Column("ID")]
    public int Id { get; set; }

    [Column("MaTV")]
    public int MaTv { get; set; }

    [Column("MaHD")]
    public int? MaHd { get; set; }

    [StringLength(1000)]
    public string NoiDung { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? NgayGopY { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("Feedbacks")]
    public virtual HoatDong? MaHdNavigation { get; set; }

    [ForeignKey("MaTv")]
    [InverseProperty("Feedbacks")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
