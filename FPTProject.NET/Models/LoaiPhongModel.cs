using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("LoaiPhong")]
[Index("MaLp", Name = "LoaiPhong_maLP", IsUnique = true)]
public partial class LoaiPhongModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maLP")]
    public int MaLp { get; set; }

    [Column("tenLP")]
    [StringLength(50)]
    public string TenLp { get; set; } = null!;

    [Column("donGia", TypeName = "decimal(19, 0)")]
    public decimal? DonGia { get; set; }

    public virtual ICollection<PhongModel> Phongs { get; } = new List<PhongModel>();
}
