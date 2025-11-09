using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("BanChuyenMon")]
public partial class BanChuyenMon
{
    [Key]
    public int MaBan { get; set; }

    [StringLength(100)]
    public string TenBan { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    public int? TruongBan { get; set; }

    [InverseProperty("MaBanNavigation")]
    public virtual ICollection<BanChuyenMonThanhVien> BanChuyenMonThanhViens { get; set; } = new List<BanChuyenMonThanhVien>();

    [InverseProperty("MaBanNavigation")]
    public virtual ICollection<ThanhVien> ThanhViens { get; set; } = new List<ThanhVien>();

    [ForeignKey("TruongBan")]
    [InverseProperty("BanChuyenMons")]
    public virtual ThanhVien? TruongBanNavigation { get; set; }
}
