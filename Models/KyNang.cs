using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("KyNang")]
public partial class KyNang
{
    [Key]
    [Column("MaKN")]
    public int MaKn { get; set; }

    [Column("TenKN")]
    [StringLength(150)]
    public string TenKn { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [StringLength(50)]
    public string? CapDo { get; set; }

    [InverseProperty("MaKnNavigation")]
    public virtual ICollection<ThanhVienKyNang> ThanhVienKyNangs { get; set; } = new List<ThanhVienKyNang>();
}
