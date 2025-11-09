using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[PrimaryKey("MaLh", "MaTv")]
[Table("DiemDanhLichHop")]
public partial class DiemDanhLichHop
{
    [Key]
    [Column("MaLH")]
    public int MaLh { get; set; }

    [Key]
    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(20)]
    public string? TrangThai { get; set; }

    [ForeignKey("MaLh")]
    [InverseProperty("DiemDanhLichHops")]
    public virtual LichHop MaLhNavigation { get; set; } = null!;

    [ForeignKey("MaTv")]
    [InverseProperty("DiemDanhLichHops")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
