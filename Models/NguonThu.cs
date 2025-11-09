using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("NguonThu")]
public partial class NguonThu
{
    [Key]
    public int MaNguon { get; set; }

    [StringLength(150)]
    public string TenNguon { get; set; } = null!;

    [StringLength(255)]
    public string? MoTa { get; set; }

    [InverseProperty("MaNguonNavigation")]
    public virtual ICollection<ThuChi> ThuChis { get; set; } = new List<ThuChi>();
}
