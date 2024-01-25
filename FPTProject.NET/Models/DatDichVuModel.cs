using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Hotel.Models;

[Table("DatDichVu")]
[Index("MaDdv", Name = "DatDichVu_maDDV", IsUnique = true)]
public partial class DatDichVuModel
{
    [Key]
    [Column("id")]
    public int Id { get; set; }

    [Column("maDDV")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDdv { get; set; } = null!;

    [Column("soLuong")]
    public int SoLuong { get; set; }

    [Column("ngayDatDichVu", TypeName = "datetime")]
    public DateTime NgayDatDichVu { get; set; }

    [Column("maDV")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDv { get; set; } = null!;

    [Column("maDP")]
    [StringLength(10)]
    [Unicode(false)]
    public string MaDp { get; set; } = null!;

    [Column("tongTien", TypeName = "decimal(12, 0)")]
    public decimal? TongTien { get; set; }

    [Column("ngayHD", TypeName = "datetime")]
    public DateTime? NgayHd { get; set; }

    public virtual DatPhongModel? MaDpNavigation { get; set; } = null!;

    public virtual DichVuModel? MaDvNavigation { get; set; } = null!;
}
