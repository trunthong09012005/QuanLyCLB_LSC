using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("ThuChi_ChiTiet")]
public partial class ThuChiChiTiet
{
    [Key]
    [Column("MaCT")]
    public int MaCt { get; set; }

    [Column("MaGD")]
    public int MaGd { get; set; }

    [StringLength(500)]
    public string? NoiDung { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal SoTien { get; set; }

    [ForeignKey("MaGd")]
    [InverseProperty("ThuChiChiTiets")]
    public virtual ThuChi MaGdNavigation { get; set; } = null!;
}
