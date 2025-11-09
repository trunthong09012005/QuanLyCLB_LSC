using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Keyless]
public partial class VwDangKyChoDuyet
{
    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(150)]
    public string HoTen { get; set; } = null!;

    [Column("MaHD")]
    public int MaHd { get; set; }

    [Column("TenHD")]
    [StringLength(200)]
    public string TenHd { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? ThoiGianDangKy { get; set; }

    [StringLength(50)]
    public string? TrangThai { get; set; }
}
