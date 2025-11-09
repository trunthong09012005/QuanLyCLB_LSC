using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("DiemRenLuyen")]
public partial class DiemRenLuyen
{
    [Key]
    [Column("MaDRL")]
    public int MaDrl { get; set; }

    [Column("MaTV")]
    public int MaTv { get; set; }

    [StringLength(20)]
    public string? HocKy { get; set; }

    [StringLength(20)]
    public string? NamHoc { get; set; }

    public int? Diem { get; set; }

    [ForeignKey("MaTv")]
    [InverseProperty("DiemRenLuyens")]
    public virtual ThanhVien MaTvNavigation { get; set; } = null!;
}
