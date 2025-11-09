using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("ChucVu")]
public partial class ChucVu
{
    [Key]
    [Column("MaCV")]
    public int MaCv { get; set; }

    [Column("TenCV")]
    [StringLength(100)]
    public string TenCv { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("MaCvNavigation")]
    public virtual ICollection<ThanhVien> ThanhViens { get; set; } = new List<ThanhVien>();
}
