using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace QuanLyCLB_LSC.Models;

[Table("VaiTro")]
public partial class VaiTro
{
    [Key]
    public int MaVaiTro { get; set; }

    [StringLength(50)]
    public string? TenVaiTro { get; set; }

    [StringLength(200)]
    public string? MoTa { get; set; }
}
