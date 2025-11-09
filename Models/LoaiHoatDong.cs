using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("LoaiHoatDong")]
public partial class LoaiHoatDong
{
    [Key]
    [Column("MaLoaiHD")]
    public int MaLoaiHd { get; set; }

    [Column("TenLoaiHD")]
    [StringLength(100)]
    public string TenLoaiHd { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("MaLoaiHdNavigation")]
    public virtual ICollection<HoatDong> HoatDongs { get; set; } = new List<HoatDong>();
}
