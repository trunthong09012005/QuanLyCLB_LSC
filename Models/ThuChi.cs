using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("ThuChi")]
public partial class ThuChi
{
    [Key]
    [Column("MaGD")]
    public int MaGd { get; set; }

    [Column("LoaiGD")]
    [StringLength(10)]
    public string? LoaiGd { get; set; }

    [Column(TypeName = "money")]
    public decimal SoTien { get; set; }

    [Column("NgayGD")]
    public DateTime? NgayGd { get; set; }

    [StringLength(500)]
    public string? NoiDung { get; set; }

    public int? NguoiThucHien { get; set; }

    [Column("MaHD")]
    public int? MaHd { get; set; }

    public int? MaNguon { get; set; }

    [ForeignKey("MaHd")]
    [InverseProperty("ThuChis")]
    public virtual HoatDong? MaHdNavigation { get; set; }

    [ForeignKey("MaNguon")]
    [InverseProperty("ThuChis")]
    public virtual NguonThu? MaNguonNavigation { get; set; }

    [ForeignKey("NguoiThucHien")]
    [InverseProperty("ThuChis")]
    public virtual ThanhVien? NguoiThucHienNavigation { get; set; }

    [InverseProperty("MaGdNavigation")]
    public virtual ICollection<ThuChiChiTiet> ThuChiChiTiets { get; set; } = new List<ThuChiChiTiet>();
}
